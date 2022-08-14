﻿using System;
using System.Text.RegularExpressions;
using System.IO;  
using System.Collections.Generic;
using System.Linq;

//program to read EDI file and parse out fields and elements for CLM tickets, and print out result
namespace EDI_Parse_Visual
{
    class Program
    {

        //start by getting the current directory for now
        //created an Input_File directory to include for cleaner operation

        static string path = $"{Directory.GetCurrentDirectory()}/Input_File/";

        public static string textFile;
        
        //create a list for calculating total claim charge amounts
        public static List<Int32> totClaimChargeAmount = new List<Int32>();

        //create an array for calculating total claim charge amounts
        //public static int[] totClaimChargeAmount;

        //create a claimCount to calculate total claim tickets
        public static int claimCount = 0;

        static void fileInput()
        {
            //I'm coding this in linux so no access to windows forms
            //So we'll go command line
            System.Console.WriteLine($"Please place the file in {path}");
            System.Console.WriteLine($"Then hit enter.");
          
            //readline to halt the program for file movement
            System.Console.ReadLine();

            System.Console.WriteLine($"Please type the input filename from this list.");

            //Here, we print out all the filenames in the current path
            foreach(var path1 in Directory.GetFileSystemEntries(path))
                {
                Console.WriteLine(System.IO.Path.GetFileName(path1)); // full path
                }

            //now we read the input file from user input; if it doesn't exist, we'll loop back
            textFile = $"Input_File/{System.Console.ReadLine()}";

        }

        //create an array for the output, a little more elegant this way
        static string[] elementList = 
        {
            "Segment Identifier: ",
            "CLM01 Element (Represents Patient Account Number): ",
            "CLM02 Element (Represents Total Claim Charge Amount): ",
            "CLM03 Element (Not Used): ",
            "CLM04 Element (Not Used): ",
            "CLM05 - 1 Composite Element (Represents Facility Type Code): ",
            "CLM05 - 2 Composite Element (Represents Facility Code Qualifier): ",
            "CLM05 - 3 Composite Element (Represents Claim Frequency Code): ",
            "CLM06 (Represents Provider/Supplier Signature Indicator): ",
            "CLM07 (Represents Assignment/Plan Participation Code): ",
            "CLM08 (Represents Benefits Assignment Certification Indicator): ",
            "CLM09 (Represents Release of Information Indicator): ",
            
        };

        static void Main(string[] args)
        {
            //for readability, linebreak between initialization and output
            System.Console.WriteLine("");
            //run fileInput command
            fileInput();

            //just for readability, put a line break between file input and tickets
            System.Console.WriteLine("");

            //if they enter something invalid, prompt to enter valid filename and loop back to file input
            while(!File.Exists(textFile))
            {
                System.Console.WriteLine("Please enter a valid filename.");
                fileInput();
            }

            //check to see if the file exists first
            if (File.Exists(textFile)) 
            {  
                // Read entire text file content in one string and store in string variable
                string inputFile = File.ReadAllText(textFile);  

                //we have a segment delimiter, so remove the line breaks
                inputFile = inputFile.Replace(System.Environment.NewLine, "");

                //next we'll take the text and split it by segment, using the segment delimiter ~
                string[] segmentArray = inputFile.Split('~');

                //create a ticketCount to delineate different tickets in the output
                int ticketCount = 0;

                //foreach loop; loop through file data segment by segment
                foreach (var segment in segmentArray)
                {
                    //create an elementCount variable to tell which element is which, reset to 0
                    int elementCount = 0;

                    //initialize outputText string
                    string outputText;
                    //start by incrementing the ticketCount
                    ticketCount++;

                    //we have element delimiters and composite element separators
                    //so let's just replace the composite element separators with delimiters for now
                    string segment2 = Regex.Replace(segment, "\\:", "*");

                    //now we split the segments into elements and store in elementArray, using element delimter (*)
                    string[] elementArray = segment2.Split('*');

                    //we only want claim tickets, so continue if the first element is anything else
                    if (elementArray[0] != "CLM")
                        continue;

                    //foreach adds an extra ticket, so just break if we hit the length
                    if (ticketCount == segmentArray.Length)
                        break;
                    //output the ticket number
                    System.Console.WriteLine($"Reading ticket #{ticketCount}");
                    System.Console.WriteLine("");

                    //for each element, we output it in a new line
                    foreach (string element in elementArray)
                    {
                        if (elementCount == 2)
                        {
                            //if elementCount is 0, we use substring to remove some invisible characters 
                            //that break things, and Concat to put the final string together
                            string fixString = string.Concat(elementList[elementCount], element);
                            System.Console.WriteLine($"{fixString}");

                            try
                            {
                                int elementInt = Int32.Parse(element);
                                totClaimChargeAmount.Add(elementInt);
                            }
                            catch (FormatException)
                            {
                                Console.WriteLine($"Unable to parse '{element}'");
                            }
                            // Output: Unable to parse ''
                            

                        }
                        //if the element count is within the bounds of the elementList, use it
                        else if (elementCount < 12)
                        {   
                            string fixString = string.Concat(elementList[elementCount], element );
                            String.Format(fixString,fixString);
                            System.Console.WriteLine($"{fixString}");
                        }
                        else
                        {
                            //if the count is outside the bounds of our array, just dynamically create new descriptions
                            int elementCount2 = elementCount -11;
                            outputText =  "Additional Element #" + (elementCount2) + ": " + element;
                        }

                        elementCount++;
                    }

                    //increment claimCount
                    claimCount++;

                    //just for readability, put a line break between each ticket
                    System.Console.WriteLine("");
                }

            }  
            System.Console.WriteLine($"Total claim tickets processed: {claimCount}");
            try
            {
                int totClaimChargeSum = totClaimChargeAmount.Sum();
                System.Console.WriteLine($"Total Claim Charge Amount: {totClaimChargeSum}");
            }
            catch
            {
                System.Console.WriteLine("Failed to summarize claim charge total. Please verify the values.");
            }
            System.Console.WriteLine("");
            System.Console.WriteLine("Thanks for trying my application! Press Enter to close.");
            System.Console.WriteLine("Created by Wil C. Dunlap");
            System.Console.ReadLine();
        }
    }
}
