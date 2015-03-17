#checks if miniVNA jar file is present
#as of March 17, 2015.... using vnaJ headless v3.1.3
#see website for updated versions: http://vnaj.dl2sba.com/
if [ ! -f vnaJ-hl.3.1.3.jar ];
then
    echo ERROR! Missing vnaJ file...
    exit
fi

#checks if miniVNA PRO is connected
if [ -z $(lsusb | grep -e "Future Technology Devices") ];
then
    echo ERROR! MiniVNA Pro not connected...
fi

#checks if parameters for antenna stop and start are set when script is executed
#ex: gbin.sh tx_start tx_stop rx_start rx_stop
#note: probes not implemented here
if [ $# -lt 4 ];
then
    echo ERROR! Missing parameters...
    exit
fi

#checks if Arduino from RF Switch is connected, else run data acquisition process
if [ -z $(lsusb | grep -e "Arduino") ];
then
     echo ERROR! Arduino not connected...
else
    echo Tx: $1 - $2
    echo Rx: $3 - $4
    stty -F /dev/ttyACM0 cs8 9600 ignbrk -brkint -imaxbel -opost -onlcr -isig -icanon -iexten -echo -echoe -echok -echoctl -echoke noflsh -ixon -crtscts -hupcl

    for i in $(seq $1 $2)
    do
		#sending transmitter no. to Arduino (if single digit, add 0 in front, else send 2 digit number)
        if [ "$i" -lt 10 ];
        then
            echo sending 0$i to arduino...
            echo -n "0$i" > /dev/ttyACM0
        else
            echo sending $i to arduino...
            echo -n "$i" > /dev/ttyACM0
        fi
		#response from Arduino (did not work during tests)
        #reply=$(cat /dev/ttyACM0)
        #if [ $reply = $i ];
        #then
            echo changing transmitter to $i
        #fi

        for j in $(seq $3 $4)
        do
			#sending receiver no. to Arduino
            echo -n "$j" > /dev/ttyACM0
			#response from Arduino (did not work during tests)
            #reply=$(cat /dev/ttyACM0)
            #if [ $reply = $j ]
            #then
                echo changing reciever to $j
            #fi

            echo Running vnaJ-hl.3.1.3.jar...
            #sleep 5
			#see log.txt for jar file output from miniVNA software
            nohup java -Dconfigfile=gbin.xml -Dfstart=70000000 -Dfstop=100000000 -Dfsteps=100 -Dcalfile=gbin.cal -Dscanmode=TRAN -Dexports=csv -jar vnaJ-hl.3.1.3.jar > log.txt
       
			path="vnaJ.3.1/export"
            #rename files for processing; "gbin_txrx.csv" where tx and rx are the antenna numbers (1-24 in this case)
			[ "$i" -lt 10 ] && tx="0$i"|| tx="$i"
            rcvr=$(expr $j - 24)
            [ "$rcvr" -lt 10 ] && rx="0$rcvr"|| rx="$rcvr"
            mv ${HOME}/$path/gbin.cal.csv /${HOME}/$path/gbin_"$tx$rx".csv
        done
    done
fi

#runs post-data processing procedure
mono put2str.exe

#removes exported data files from miniVNA PRO after processing
#note sp.dat output file from post-data processing procedure is in folder /root/grainbin/output on RPi2
rm -R ${HOME}/$path/*

#upload sp.dat to dropbox
#note that dropbox account must be linked prior to running this script
./dropbox_uploader.sh upload /root/grainbin/output/* ./
