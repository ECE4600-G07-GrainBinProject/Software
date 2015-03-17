import RPi.GPIO as GPIO
from time import sleep
from sys import exit
import os

# to use Raspberry Pi board pin numbers
# please refer to Raspberry Pi pinout: http://pi.gadgetoid.com/pinout
GPIO.setmode(GPIO.BCM)

# set up the GPIO channels - one input and one output
GPIO.setup(17, GPIO.IN) 	#push button set to input
GPIO.setup(23, GPIO.OUT) 	#LED set to output

try:
  while True:
	#LED should be blinking if there is no button press
	GPIO.output(23, True)
	sleep(0.5)
	GPIO.output(23, False)
	sleep(0.5)
	
	#if button pressed (execute script)
	if(GPIO.input(17) == True):
	 #executes gbin.sh
	 #parameters are set to have only 1 transmitter (port 18)
	 #and receivers 45-48 (port 21-24)
	 os.system("sh gbin.sh 18 18 45 48")
	#else:
	#print("OFF!")
      
finally: GPIO.cleanup()
