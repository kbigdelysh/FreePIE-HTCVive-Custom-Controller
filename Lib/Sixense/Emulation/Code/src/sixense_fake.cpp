#include "Lib/sixense.h"
#include <stdint.h>
#include <xtgmath.h>
#include "Lib\sixense_math.hpp"

#include <iostream>
#include <fstream>
#include <vector>
#include <array>
#include "shared_memory.h"
#include <algorithm>

std::array<std::vector<sixenseControllerData>, 2> controller_data;

struct emulated_data
{
  float yaw, pitch, roll;
  float x, y, z;
  float joystick_x;
  float joystick_y;
  float trigger;
  unsigned int buttons;
  int enabled;
  int controller_index;
  unsigned char is_docked;
  unsigned char which_hand;
};

freepie_io::shared_memory<std::array<emulated_data, 2>> shared_memory;

SIXENSE_EXPORT int sixenseInit( void )
{
  shared_memory = freepie_io::shared_memory<std::array<emulated_data, 2>>("SixenseEmulatedData");

  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseExit( void )
{
  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseGetMaxBases()
{
  return 4;
}
SIXENSE_EXPORT int sixenseSetActiveBase( int i )
{
  return SIXENSE_SUCCESS;
}
SIXENSE_EXPORT int sixenseIsBaseConnected( int i )
{
  if(i == 0)
    return 1;
  return 0;
}

SIXENSE_EXPORT int sixenseGetMaxControllers( void )
{
  return 4;
}

SIXENSE_EXPORT int sixenseIsControllerEnabled( int which )
{
  if(which <= 1)
    return 1;
  return 0;
}
SIXENSE_EXPORT int sixenseGetNumActiveControllers()
{
  return 2;
}

SIXENSE_EXPORT int sixenseGetHistorySize()
{
  return 0;
}

void convert_euler(float yaw, float pitch, float roll, sixenseControllerData *output) {
  auto mat =
    sixenseMath::Matrix3::rotation( roll,  sixenseMath::Vector3( 0,  0, -1 ) ) * 
    sixenseMath::Matrix3::rotation( pitch, sixenseMath::Vector3( 1,  0,  0 ) ) *
    sixenseMath::Matrix3::rotation( yaw,   sixenseMath::Vector3( 0, -1,  0 ) );
  auto quat = sixenseMath::Quat(mat);
  
  quat.fill((float*)&output->rot_quat);
  mat.fill((float(*)[3])&output->rot_mat);  
 }

std::array<uint8_t, 2> sequence_numbers = { 0 };

SIXENSE_EXPORT int sixenseGetData(int which, int index_back, sixenseControllerData *output)
{
  if(index_back != 0 || which > 2)
    return SIXENSE_FAILURE;

  auto view = shared_memory.open_view();

  auto data = view.map()[which];

  output->sequence_number = sequence_numbers[which]++;

  output->buttons = data.buttons;
  output->trigger = data.trigger;
  output->controller_index = which;
  output->which_hand = data.which_hand;
  output->enabled = data.enabled;
  output->firmware_revision = 174;
  output->hardware_revision = 0;
  output->is_docked = data.is_docked;
  output->joystick_x = data.joystick_x;
  output->joystick_y = data.joystick_y;
  output->pos[0] = data.x;
  output->pos[1] = data.y;
  output->pos[2] = data.z;
  output->hemi_tracking_enabled = 1;
  output->magnetic_frequency = 0;
  output->packet_type = 1;

  convert_euler(data.yaw, data.pitch, data.roll, output);

  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseGetAllData( int index_back, sixenseAllControllerData *output)
{
  auto success = sixenseGetData(0, index_back, &output->controllers[0]);
  success |= sixenseGetData(1, index_back, &output->controllers[1]);
  return success;
}
SIXENSE_EXPORT int sixenseGetNewestData( int which, sixenseControllerData *output)
{
  return sixenseGetData(which, 0, output);
}

SIXENSE_EXPORT int sixenseGetAllNewestData( sixenseAllControllerData *output)
{
  return sixenseGetAllData(0, output);
}

SIXENSE_EXPORT int sixenseSetHemisphereTrackingMode( int which_controller, int state )
{
  return SIXENSE_SUCCESS;
}
SIXENSE_EXPORT int sixenseGetHemisphereTrackingMode( int which_controller, int *state )
{
  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseAutoEnableHemisphereTracking( int which_controller )
{
  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseSetHighPriorityBindingEnabled( int on_or_off )
{ 
  return SIXENSE_SUCCESS;
}
SIXENSE_EXPORT int sixenseGetHighPriorityBindingEnabled( int *on_or_off )
{
  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseTriggerVibration( int controller_id, int duration_100ms, int pattern_id )
{
  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseSetFilterEnabled( int on_or_off )
{
  return SIXENSE_SUCCESS;
}
SIXENSE_EXPORT int sixenseGetFilterEnabled( int *on_or_off )
{
  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseSetFilterParams( float near_range, float near_val, float far_range, float far_val )
{
  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseGetFilterParams( float *near_range, float *near_val, float *far_range, float *far_val )
{
  return SIXENSE_SUCCESS;
}

SIXENSE_EXPORT int sixenseSetBaseColor( unsigned char red, unsigned char green, unsigned char blue )
{
  return SIXENSE_SUCCESS;
}
SIXENSE_EXPORT int sixenseGetBaseColor( unsigned char *red, unsigned char *green, unsigned char *blue )
{
  return SIXENSE_SUCCESS;
}