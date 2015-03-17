using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;

class put2string{
	
  //data that was processed from the exported miniVNA PRO files	
  static string[,] sp = new string[256, 3];

  public static void Main(string[] args)
  {
	  //path of exported miniVNA PRO data files on RPi2
      string path = @"/root/vnaJ.3.1/export";
		
	  //verify path exists on RPi2
      if(Directory.Exists(path))
          ProcessDirectory(path); //process files in export directory of RPi2
      else
          Console.WriteLine("{0} is not a valid directory.", path);
		
	  //process array from sp array to write to sp.dat file	
      List<string> linesToWrite = new List<string>();
      for(int rowIndex = 0; rowIndex < 256; rowIndex++)
      {
          StringBuilder line = new StringBuilder();
          for(int colIndex = 0; colIndex < 3; colIndex++)
              line.Append(sp[rowIndex, colIndex]).Append("\t");
          linesToWrite.Add(line.ToString());
      }

      //export file to sp.dat
	  //may want to implement a time stamp for the name here so that the file does not get overwritten
	  //with multiple scans
      System.IO.File.WriteAllLines(@"/root/grainbin/output/sp.dat", linesToWrite.ToArray());
  }

    // Process all files in the directory passed in
    public static void ProcessDirectory(string targetDirectory)
    {
        // Process the list of files found in the directory.
        string [] fileEntries = Directory.GetFiles(targetDirectory, "*.csv");
        if(fileEntries.Length == 0)
	    Console.WriteLine("ERROR! No files in directory to process");
	else{
		int count = 0;
        	foreach(string fileName in fileEntries)
        	{
				//retrieves tx and rx number from file name of miniVNA PRO exported data file
				string dataID = Path.GetFileNameWithoutExtension(fileName);
				string tx = dataID.Substring(dataID.Length-4,2);
				string rx = dataID.Substring(dataID.Length-2,2);
				sp[count, 0] = tx;
				sp[count, 1] = rx;
				// Console.WriteLine("TX: {0}\tRX: {1}", sp[count, 0], sp[count, 1]);
				ProcessFile(fileName, count);
				count++;
        	}
	}
    }

    // Insert logic for processing found files here.
    public static void ProcessFile(string file, int count)
    {
        string[] lines = System.IO.File.ReadAllLines(file);
        string data = "";
        foreach(string line in lines.Skip(1)){

          string[] val = line.Split(',');
			
		  //converting magnitude and phase to real and imaginary
		  //note that phase is in degrees not radians so use proper 
		  //cos and sin functions to handle degrees
          double magdb = Convert.ToDouble(val[1]);
          double ph = Convert.ToDouble(val[2]);
          double mag = Math.Pow(10, magdb/20);
          double a = mag*Math.Cos(Math.PI*ph/180);
          double b = mag*Math.Sin(Math.PI*ph/180);

          data = string.Concat(data, string.Concat(a.ToString("N7") + "\t", b.ToString("N7") + "\t"));

          sp[count, 2] = data;
        }

        // Console.WriteLine("Processed file '{0}'.", file);
    }
  }
