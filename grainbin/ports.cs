using System;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Linq;

/*[StructLayout(LayoutKind.Sequential)]
public class MyComplex {
    public double re;
    public double im;
}*/

public class SerialPortVNA{
    //courtesy of Karl Jan Skontorp
    //[DllImport("miniVNAPro_dll.dll")]
    //public static extern MyComplex GetMeasuredData(byte b0, byte b1, byte b2, byte b3, byte b4,
    //  byte b5, byte b6, byte b7);


    public static void Main(string[] args)
    {
		//time process
        Stopwatch sw = Stopwatch.StartNew();
		
		//when runnning c# program, the port name of the miniVNA PRO must be set as an argument
        if(args.Length < 1){
          Console.WriteLine("Error! Please enter in port name as argument.");
          System.Environment.Exit(0);
        }
		
        Console.WriteLine("Port Name: "+ args[0]);

        SerialPortVNA myTest = new SerialPortVNA();
        myTest.Test(args[0]);
	
        //time execution
        sw.Stop();
        Console.WriteLine("Time taken: {0}ms", sw.Elapsed.TotalMilliseconds);
    }

    private SerialPort mySerial;

    // Constructor
    public SerialPortVNA()
    {
    }

    public void Test(string portName)
    {
		//initialize miniVNA PRO port
        mySerial = new SerialPort(portName);
        if (mySerial != null)
            if (mySerial.IsOpen)
                mySerial.Close();

        mySerial = new SerialPort(portName, 115200, 0, 8);
        try{
          mySerial.Open();
        }
        catch{
          Console.WriteLine("\nError! Unable to open " + portName);
        }

        //mySerial.RtsEnable = true;
        //mySerial.DtrEnable = true;

        //set timeouts
        mySerial.ReadTimeout = 500;
        mySerial.WriteTimeout = 500;


        /*
        Command example: Reflection Mode, 0.1MHz to 200MHz, 1001 steps and 520.000850DDSClockMain
        1
        825955
        0
        1001
        1649435
        */

        int mode = 1;  //mode: 0 - transmission, 1 - reflection
        int start = 825955;
        int phase = 0;
        int steps = 5;
        int stop = 1649435;

        //commands are sent to miniVNA PRO
        mySerial.DiscardInBuffer();
        if(SendData(mode.ToString()))
          Console.WriteLine(mode + " sent...");
        else
          SendData(mode.ToString());
        System.Threading.Thread.Sleep(20);

        mySerial.DiscardInBuffer();
        if(SendData(start.ToString()))
          Console.WriteLine("825955 sent...");
        else
          SendData(start.ToString());
        System.Threading.Thread.Sleep(20);

        mySerial.DiscardInBuffer();
        if(SendData(phase.ToString()))
          Console.WriteLine("0 sent...");
        else
          SendData(phase.ToString());
        System.Threading.Thread.Sleep(20);

        mySerial.DiscardInBuffer();
        if(SendData(steps.ToString()))
          Console.WriteLine(steps + " sent...");
        else
          SendData(steps.ToString());
        System.Threading.Thread.Sleep(20);

        mySerial.DiscardInBuffer();
        if(SendData(stop.ToString()))
          Console.WriteLine("1649435 sent...");
        else
          SendData(stop.ToString());
        System.Threading.Thread.Sleep(20);


        //check for bytes to read
        while(mySerial.BytesToRead == 0){
          //Console.WriteLine("No bytes to read...");
          System.Threading.Thread.Sleep(1000);
        }

        //double[,] data = new double[steps,2];
        byte[] recBytes = ReadData(steps);

        string path = Directory.GetCurrentDirectory();
        //uint result = BitConverter.ToUInt32(recBytes, 0);
        string hex = BitConverter.ToString(recBytes);
        string[] bin = recBytes.Select(x => Convert.ToString(x,2).PadLeft( 8, '0' )).ToArray();
        System.IO.File.WriteAllText(path+@"/test.txt", hex);
        System.IO.File.WriteAllLines(path+@"/testBin.txt", bin);

        //process received bytes from miniVNA (not yet implemented due to dll not compatible with LINUX
        /*MyComplex dataPoint= new MyComplex();
        while(i<8*steps){
          dataPoint = GetMeasuredData(recBytes[i],recBytes[i+1],recBytes[i+2],
            recBytes[i+3],recBytes[i+4],recBytes[i+5],recBytes[i+6],recBytes[i+7]);
          data[j,0]  = dataPoint.im;
          data[j,1] = dataPoint.re;
          j++;
          Console.WriteLine(dataPoint.re + "\t" + dataPoint.im);
        }*/
        mySerial.Close();

    }
	
	//process returned bytes from miniVNA PRO and stores to array
    public byte[] ReadData(int steps)
    {
        int recBytes = 0;
        int bytes2read = (8*steps);
        byte[] measuredData = new byte[bytes2read];
        int count = 0;

        while(recBytes < bytes2read)
        {
          byte[] data = new byte[mySerial.BytesToRead];
          recBytes += mySerial.BytesToRead;
          mySerial.Read(data, 0, mySerial.BytesToRead);

          for(int j=0;j<data.Length;j++){
            measuredData[count] = data[j];
            count++;
            //Console.WriteLine(count);
          }
          //Console.WriteLine(recBytes + " bytes read...\n");
        }

        Console.WriteLine("Total Bytes read = " + recBytes);
        return measuredData;
    }
	
	//simple method to send command to miniVNA PRO port
    public bool SendData(string Data)
    {
        mySerial.Write(' ' + Data + '\r');
        return true;
    }
}
