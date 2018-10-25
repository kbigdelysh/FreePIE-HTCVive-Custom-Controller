global pressed
def handleStartButton():
  global pressed
  if pressed == 0:
    if keyboard.getKeyDown(Key.Space): # For holding up two system buttons (start buttons) at the same time. 
    								   # This leads to setting the controllers to HMD position. 
      hydra[0].start = True
      hydra[1].start = True
      pressed = 1
    elif keyboard.getKeyDown(Key.Backspace): # For clicking the start button (system button) to bring up the steam menu.
      hydra[0].start = True
      pressed = 1
  elif keyboard.getKeyDown(Key.Backspace):    # For situations in which the start button (system button) is 
                                              # hold down to bring the turn off menu).
    pressed = 1
  else:
    hydra[0].start = False
    hydra[1].start = False
    pressed = 0

def init_hydra(index):
  if index == 0:
    hydra[index].x = 65
    hydra[index].y = -45
    hydra[index].z = -200
    hydra[index].side = 'R'
  else:
    hydra[index].x = -65
    hydra[index].y = -45
    hydra[index].z = -200
    hydra[index].side = 'L'    
  hydra[index].yaw = 0
  hydra[index].pitch = 0
  hydra[index].roll = 0
  hydra[index].start = True
  hydra[index].isDocked = False
  hydra[index].enabled = True
  hydra[index].trigger = 0
  hydra[index].three = 0
  hydra[index].four = 0
  hydra[index].one = 0
  hydra[index].two = 0
  hydra[index].bumper = 0
  hydra[index].joybutton = 0
  hydra[index].joyx = 0
  hydra[index].joyy = 0
  
def vive_controllers_init():
 init_hydra(0)
 init_hydra(1)
 
def update():
 handleStartButton()
 diagnostics.watch(myArduino.px)
 diagnostics.watch(myArduino.py)
 diagnostics.watch(myArduino.pz)

 diagnostics.watch(myArduino.roll)
 diagnostics.watch(myArduino.pitch)
 diagnostics.watch(myArduino.yaw)

 index = 0 # there are two Vive controllers. Here we choose the one with index '0' to represent our arduino.
 hydra[index].x = myArduino.px
 hydra[index].y = myArduino.py
 hydra[index].z = myArduino.pz
 
 hydra[index].roll = myArduino.roll
 hydra[index].pitch = myArduino.pitch
 hydra[index].yaw = myArduino.yaw; 
 
if starting:
  pressed = 0
  vive_controllers_init()

update()