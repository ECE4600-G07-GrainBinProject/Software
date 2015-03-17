# Software
Grain Bin Project Work Files

Both the grainbin and vnaJ.3.1 folders need to be on the root directory of the RPi2.  

The current setup starts by running the button.py script: python /root/grainbin/button.py

To setup button.py to run at bootup of the RPi2, enter this on the RPi2 when logged into ARCH LINUX (can access RPi2 either through HDMI cable or SSH)
1. crontab -e
2. @reboot python /home/grainbin/button.py

NOTE: Login information for ARCH LINUX is,

username: root
password: gbin2015

The static IP address is set to be 192.168.1.23 when an ethernet cable is connected (this is handy to ssh but is not reliable so HDMI output is the way to go).

To create calibration files, please use the vnaJ GUI and follow user manual. 
Available: http://vnaj.dl2sba.com/index.php?option=com_content&view=article&id=11&Itemid=118

