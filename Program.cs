//trying project writting in c# 
// CSC563 Project #1 : Benchmark Application by Brian Pagoni 

using System;
using System.Threading;
using System.Diagnostics;
class Threads
{
    
    static void createThreads(int numThreads, string operation)
    {   for (int i = 1; i < numThreads; i++)
        {
            ParameterizedThreadStart parameterizedThreadStart = new ParameterizedThreadStart(useThreads);
            Thread thread = new Thread(parameterizedThreadStart);
            thread.Start(operation);
        }
    }
    static void useThreads(object operation)
    { string operationName = (string)operation;
        if (operationName == "")
            {
                throw new ArgumentNullException(nameof(operation));
            }
        if (operationName == "setDuration")
            {
                
            }
        else
            {
                
        } 
    }

    static void doCalculations(int testType)
    {
        double[,] resultsTime = new double[2, 3]; //making a 2d array of results, the first element is for FLOPS operations, the second element is for IOPS operations 
        double[,] resultsOps = new double[2, 3]; //making a 2d array of results, the first element is for FLOPS operations, the second element is for IOPS operations 

       for (int i = 0; i < 3; i++)
        {
            switch (testType) { 
                case 0:
                        double flopsTime = GetTime(() => 1.0f + 2.0f);
                        resultsTime[0, i] = flopsTime;
                        double iopsTime = GetTime(() => 1 + 2);
                        resultsTime[1, i] = iopsTime;
                    break;
                case 1: { 
                
                    
                        double flopsOps = GetOps(() => 1.0f + 2.0f);
                        resultsOps[0, i]= flopsOps;
                        double intOps = GetOps(() => 1 + 2);
                        resultsOps[1,i] = intOps;
                        break;
                               }

            } 
            }
    }
    static double GetTime(Func<double> opType)
    {
        int numOps = 0;
        int setTime = 1_000_000;   //using milliseconds which will be 1000 seconds 
       
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            while (sw.ElapsedMilliseconds < setTime)
            {
            double operations = opType();
                numOps++;
            }
            sw.Stop();

            double results = (double)numOps / (setTime);

            return results;

        }
    static double GetOps(Func<double> opType)
    {
        int numOps = 0;
        int setOps = 1_000_000;   //using set number of Ops

        Stopwatch sw = Stopwatch.StartNew();
        sw.Start();
        while (numOps<setOps)
        {
            double operations = opType();
            numOps++;
        }
        sw.Stop();

        double results = (double)numOps / (sw.ElapsedMilliseconds);

        return results;

    }

}
        



    


