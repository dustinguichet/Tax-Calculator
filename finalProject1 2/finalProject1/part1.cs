﻿namespace Part1
{
    using part2;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    internal static class TaxCalculator
    {
        // Create a static dictionary field that holds a List of TaxRecords and is keyed by a string

        internal static Dictionary<string, List<TaxRecord>> taxRecords = new Dictionary<string, List<TaxRecord>>();

        // create a static constructor that:

        static TaxCalculator()
        {
            // enter a try/catch block for the entire static constructor to print out a message if an error occurs
            try
            {
                // declare a streamreader to read a file
                StreamReader sr;
                // initialize the dictionary to a newly create empty one
                taxRecords = new Dictionary<string, List<TaxRecord>>() { };
                // open the taxtable.csv file into the streamreader


                using (sr = new StreamReader(@"C:\Users\user\source\repos\finalProject1\finalProject1\taxTable.csv"))
                {
                    //Create a Variable to call ReadLine, each time you call ReadLine it will skip down to the next line.
                    string myString;
                    // loop over the lines from the streamreader
                    // Read the stream as a string, and write the string to the console. 
                    while ((myString = sr.ReadLine()) != null)
                    {

                        // constuct a taxrecord from the (csv) line in the file
                        TaxRecord record = new TaxRecord(myString);
                        //call the instance using the override ToString method from the TaxRecord Class
                        Console.WriteLine(record);
                        //if an error was found in myString, skip this line
                        if (record.stateCode == null)
                        {
                            Console.WriteLine("Line has been skipped\n" +
                                "----------------------------------------------------------------------------------");
                            continue;
                        }
                        // see if the state in the taxrecord is already in the dictionary
                        if (taxRecords.ContainsKey(record.stateCode))
                        {
                            //     if it is:  add the new tax record to the list of records in that state
                            Console.WriteLine("State Found,  adding to prexisting list\n" +
                                "------------------------------------------------------------------------------------------------------------");
                            taxRecords[record.stateCode].Add(record);
                        }
                        else
                        {
                            //     if it is not
                            //            create a new list of taxrecords
                            //            add the new taxrecord to the list
                            //            add the list to the dictionary under the state for the taxrecord
                            
                            Console.WriteLine("State not found, creating new list\n" +
                                "------------------------------------------------------------------------------------------------------------");
                            taxRecords.Add(record.stateCode, new List<TaxRecord>() { record });
                        }
                    }
                    //provide a way to get out of the loop when you are done with the file....
                    // catch any exceptions while processing each line in another try/catch block located INSIDE the loop
                    //   this way if the line in the CSV file is incorrect, you will continue to process the next line
                    // make sure the streamreader is disposed no matter what happens
                    sr.Dispose();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error at stream reader \n {e}");
            }

        }

        // create a static method (ComputeTaxFor)  to return the computed tax given a state and income

        public static decimal ComputeTaxFor(string userState, double userIncome, bool verbose)
        {
            double finalTax = 0;
            try
            {
                //  use the state as a key to find the list of taxrecords for that state
                //   throw an exception if the state is not found.
                if (!taxRecords.ContainsKey(userState))
                {
                    throw new Exception($"Tax records does not contain the users inputted state {userState}\n");
                }
                else
                {
                    //   otherwise use the list to compute the taxes
                    //  Create a variable to hold the final computed tax.  set it to 0
                    //  loop over the list of taxrecords for the state
                    //     check to see if the income is within the tax bracket using the floor and ceiling properties in the taxrecord
                    //if user selected verbose mode, print off how the tax calculation was done(Verbose)
                    if (verbose)
                    {
                        
                        

                        foreach (var list in taxRecords[userState])
                        {

                            if (userIncome >= list.ceiling)
                            {
                                //     if NOT:  (the income is greater than the ceiling)
                                //        compute the total tax for the bracket and add it to the running total of accumulated final taxes
                                //        the total tax for the bracket is the ceiling minus the floor times the tax rate for that bracket.  
                                //        all this information is located in the taxrecord
                                //        after adding the total tax for this bracket, continue to the next iteration of the loop
                                
                                finalTax += (list.ceiling - list.floor) * list.rate;
                                Console.WriteLine($"(Tax ceiling: {list.ceiling}  - Tax Floor: {list.floor}) * Tax Bracket Rate: {list.rate}" +
                                    $"\n Your running total: {finalTax}\n");
                            }
                            else if (userIncome >= list.floor && userIncome <= list.ceiling)
                            {
                                //     IF The income is within the tax bracket (the income is higher than the floor and lower than the ceiling
                                //        compute the final tax by adding the tax for this bracket to the accumulated taxes
                                //        the tax for this bracket is the income minus the floor time the tax rate for this bracket
                                //        this number is the total final tax, and can be returned as the final answer
                                
                                finalTax += (userIncome - list.floor) * list.rate;
                                Console.WriteLine($"(Your Income: {userIncome}  - Tax Floor: {list.floor}) * Tax Bracket Rate: {list.rate}" +
                                    $"\n Your running total: {finalTax}\n");
                            }
                            

                        }
                        
                    }
                    //User has selected NOT to receive a detailed report(Silent Mode)
                    else
                    {
                        foreach (var list in taxRecords[userState])
                        {

                            if (userIncome >= list.ceiling)
                            {
                                //     if NOT:  (the income is greater than the ceiling)
                                //        compute the total tax for the bracket and add it to the running total of accumulated final taxes
                                //        the total tax for the bracket is the ceiling minus the floor times the tax rate for that bracket.  
                                //        all this information is located in the taxrecord
                                //        after adding the total tax for this bracket, continue to the next iteration of the loop
                                finalTax += (list.ceiling - list.floor) * list.rate;
                            }
                            else if (userIncome >= list.floor && userIncome <= list.ceiling)
                            {
                                //     IF The income is within the tax bracket (the income is higher than the floor and lower than the ceiling
                                //        compute the final tax by adding the tax for this bracket to the accumulated taxes
                                //        the tax for this bracket is the income minus the floor time the tax rate for this bracket
                                //        this number is the total final tax, and can be returned as the final answer
                                finalTax += (userIncome - list.floor) * list.rate;
                            }
                            

                        }
                        
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error at ComputeTaxFor {e}\n" +
                    $"-------------------------------------------------------------------");
            }
            
            return Math.Round((decimal)finalTax, 2);
        }


        
    }
    /// seperation for visibility
    /// ///////////////////////////////////////////////////////
    /// 

    internal class TaxRecord
    {
        // create a TaxRecord class representing a line from the file.  It shoudl have public properties of the correct type
        // for each of the columns in the file
        
        public string stateCode;//  StateCode   (used as the key to the dictionary)
        public string state;//  State       (Full state name)
        public double floor;//  Floor       (lowest income for this tax bracket)
        public double ceiling;//  Ceiling     (highest income for this tax bracket )
        public double rate;//  Rate        (Rate at which income is taxed for this tax bracket)

       
        //  Create a ctor taking a single string (a csv) and use it to load the record

        public TaxRecord(string data)
        {
            try
            {
                //split data and store in appropriate property name
                string[] words;
                words = data.Split(',');

                this.stateCode = words[0];
                this.state = words[1];
                this.floor = double.Parse(words[2]);
                this.ceiling = double.Parse(words[3]);
                this.rate = double.Parse(words[4]);
                // define a list of states to validate the tax state
                List<string> states = new List<string>() {"AK", "AL", "AR", "AS", "AZ", "CA", "CO", "CT", "DC", "DE",
            "FL", "GA", "GU", "HI", "IA", "ID", "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI", "MN", "MO",
            "MP", "MS", "MT", "NC", "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "PR", "RI",
            "SC", "SD", "TN", "TX", "UM", "UT", "VA", "VI", "VT", "WA", "WI", "WV", "WY"};
                if (!states.Contains(this.stateCode))
                {
                    throw new Exception("Tax Bracket state not valid");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}\n");
                this.stateCode = null;
                this.state = null;
                this.floor = double.NaN;
                this.ceiling = double.NaN;
                this.rate = double.NaN;


            }
        }

        //  Create an override of ToString to print out the tax record info nicely
        public override string ToString()
        {
            return $"State Code: {this.stateCode} \n" +
                $"State:{this.state} \n" +
                $"Floor:{this.floor} \n" +
                $"Ceiling:{this.ceiling} \n" +
                $"Rate:{this.rate}";
        }

    }
    /// seperation for visibility
    /// ///////////////////////////////////////////////////////
    /// 
   
    internal class program
    {
        
        public static void Main()
        {
             
            bool prompt = true;
            
            // create an infinite loop to:
            while (prompt)
            {
                // prompt the user for a state and an income
                Console.WriteLine("Please Enter a State");
                var state = Console.ReadLine().ToUpper();
                //Check that the user entered a state with only 2 characters
                if(state.Length != 2)
                {
                    Console.WriteLine("Please enter the 2 character abbrieviation for your state. Try again...");
                    continue;
                    
                }
                Console.WriteLine("Please Enter an income");
                // validate the data
                bool incomeParse;
                string userIncome = Console.ReadLine();
                incomeParse = double.TryParse(userIncome, out double income);
                //prompt user for verbose or silent mode, create a public variable to hold the value
                Console.WriteLine("Do you wish to receive a detailed report of how your tax was calculated?(Y/N)");
                var verbose = Console.ReadLine().ToUpper();
                bool detailedReport;
                if (verbose.Contains('Y')  && verbose.Length == 1)
                {
                    detailedReport= true;
                    
                }else if (verbose.Contains('N') && verbose.Length == 1)
                {
                    detailedReport = false;
                }
                else
                {
                    Console.WriteLine("You must enter either 'Y' or 'N'...this program will restart");
                    continue;
                }
                
                // calculate the tax due and print out the total
                var finalTax = TaxCalculator.ComputeTaxFor(state, income, detailedReport);
                Console.WriteLine("Final Tax:" + finalTax);
                //Check if user wants to see The Employee tax records
                Console.WriteLine($"Now Showing list of employees, please press any key to continue..");
                Console.ReadLine();
                var newEmp = new EmployeeRecord();
                //Prompt user if they would like to sort the list of employees
                Console.WriteLine($"If you would like view a sorted list of employees please enter one of the following choices..\n" +
                    $"'STATE', 'INCOME', 'ID', 'NAME', 'TAX'\n other wise enter any key.");
                var sort = Console.ReadLine().ToUpper();
                


                foreach (Employee v in newEmp.SortedEmps(sort))
                {
                    Console.WriteLine($"ID: {v.iD} Name: {v.name} State: {v.stateCode} Income:{v.income} Tax Due: {v.taxDue}");
                }

                //prompt user for another tax record
                Console.WriteLine($"Do you want to check another tax record? (Y/N)");
                var loopAgain = Console.ReadLine().ToUpper();
                
                if (loopAgain.Contains("Y") && loopAgain.Length == 1)
                {
                    prompt = true;
                }else if (loopAgain.Contains("N"))
                {
                    prompt = false;
                }
                else
                {
                    Console.WriteLine("Answer Invalid This program will end..");
                    prompt = false;
                }
                // loop
            }
        }
    }
}
