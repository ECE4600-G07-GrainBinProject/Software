#ports.cs
get direct bytes from miniVNA PRO (no decode function yet)

#put2str.cs 
process exported data files from miniVNA PRO (did not implement probes)

#gbin.sh
runs data acquisition process (controls switching) and process data to upload to dropbox

#button.py
LED and button function of system

#vnaJ-hl.3.1.3
miniVNA headless software

#dropbox_upload.sh
For details, refer to: https://github.com/andreafabrizi/Dropbox-Uploader

#How to compile and run individual files:

##.sh
##shell script
./gbin.sh

##.py
##python script
python button.py

##.cs
##c[sharp] code
##compile##
gmcs ports.cs
##execute##
mono ports.exe 

##NOTE: must have python and mono packages installed on RPi2##
