//Rewrote project writting it in C#
// CSC563 Project #1 : Benchmark Application by Brian Pagoni 
// This project was initally coded in python, howver after finding many inconsitences with the way python interacts with threading, I switched to C#. 

using System.Diagnostics;

internal class BenchmarkClass
{
    private const int setTime = 30_000;                                      //this project will be using a set duration of time of 30 seconds(30,000 milliseconds) 



    private struct ResultsData                                          //created a structure to store data results 
    {   public string DateRan { get; set; }
        public string Operations { get; set; }
        public int ThreadCount { get; set; }
        public double Average { get; set; }
        public double StandardDeviation { get; set; }
    }
    private struct RawData                                          //created a structure to store raw data  
    {
        public string Operations { get; set; }
        public int ThreadCount { get; set; }
        public int threadIDNumber { get; set; }
        public double OpertionCount { get; set; }

    }

    private static double[] UseThreads(string operation)            //function to create and use threads 
    {
        List<RawData> rawData = new();
        Func<double>? useOperation = null;                 // creating a variable to hold the operation types. 
        int[] myThreads = { 1, 2, 4, 8 };                   //array of threads to be used 

        double[] testingResults = new double[4];            //arrary to hold results 

        if (operation == "")                                // error handling 
        {
            throw new ArgumentNullException(nameof(operation));
        }

        else
        {

            switch (operation)                                  //switch case to determine operation to use 
            {
                case "FLOPS":
                    {
                        useOperation = () => 1.0f + 2.0f;       //FLOPS
                        break;
                    }
                case "IOPS":
                    {
                        useOperation = () => 1 + 2;             //IOPS
                        break;
                    }
            }


            for (int i = 0; i < myThreads.Length; i++)          //run through the arrary of threads
            {

                int numThreads = myThreads[i];                  //get value of threads

                Thread[] threads = new Thread[numThreads];        //creating an array of thread objects
                double opCount = 0;                                //keep count of operations completed by thread
                for (int j = 0; j < threads.Length; j++)            //running the tests on each thread
                {
                    int threadIDNum = j+1;                                //keep track of the thread run number 

                    threads[j] = new Thread(() =>                       //creating the thread
                    {
                        double operations = GetTime(useOperation);      //send operation to gettime method


                        opCount += operations;                        //record the total number of operations completed from all threads 
                        StoreRawData(rawData, operation, numThreads, threadIDNum, operations);//gettign the raw data for each individual thread 
                    });
                    threads[j].Start();         //start threads

                }

                foreach (Thread thread in threads)              //run through the threads and join them so that each one is completed. 
                {   
                    thread.Join();
                }
                Console.WriteLine($"Testing {operation} using {numThreads} threads has completed!");
                
                testingResults[i] = opCount;            //add the results to the results arrary 

            }
            sendRawDatasToFile(rawData);//storing raw data to file 
            return testingResults;              //return the results.


        }
    }

    private static double GetTime(Func<double> opType)                 //testType willl be 0 setDuration of time, 1 will be set number of Operations 

    {
        int numOps = 0;                             //recording number of operations completed. 


        Stopwatch sw = Stopwatch.StartNew();        //using stopwatch to keep track of time 

        sw.Start();
        while (true)
        {
            _ = opType();                           // run operation 
            numOps++;
            if (sw.ElapsedMilliseconds >= setTime)  //when time has passed the set duration end 
            {
                sw.Stop();
                break;
            }
        }


        double results = (double)numOps / setTime; //geting the number of operations per second

        return results;                            //return results 

    }

    private static double[] CalculateAvg(double[,] results)        //method to get average 
    {
        int rows = results.GetLength(0);
        int cols = results.GetLength(1);
        double[] avgs = new double[cols]; //average of each column 


        for (int i = 0; i < cols; i++)
        {
            double sum = 0;
            double[] colValues = new double[cols];
            for (int j = 0; j < rows; j++)
            {

                sum += results[j, i];

                colValues[j] = results[j, i];
            }
            avgs[i] = (colValues.Average());

        }
        return avgs;                        //return average 
    }

    private static double[] CalculateStdDev(double[,] results, double[] avg)    //method to get standard deviation 
    {
        int rows = results.GetLength(0);
        int cols = results.GetLength(1);
        double[] stdDev = new double[cols]; //array to hold standard deviation for each col


        for (int i = 0; i < cols; i++)          //for each col in each row get standard divation 
        {
            double sumDiff = 0;
            for (int j = 0; j < rows; j++)
            {
                double diff = results[j, i] - avg[i];           //get the difference between element and average for the row. 
                sumDiff += diff * diff;                         //get the sum of differneces squared 
            }
            double deviation = sumDiff / rows;                  //
            stdDev[i] = (Math.Sqrt(deviation));

        }
        return stdDev;          //return results 
    }

    private static string sendResultsToFile(List<ResultsData> mylist)
    {
        int testNum = 1;
        string filePath = "";
        bool createdFile = false;
         
        while (createdFile == false)
        {
            filePath = $"Results #{testNum}.txt";//saving results to a file that has the number for the test 
            if (File.Exists(filePath))
            {
                Console.WriteLine($"{filePath} already exists in {Environment.CurrentDirectory}, attempting to add new test results txt file");
                testNum++;

            }

            else
            {

                using StreamWriter writefile = new(filePath);
                foreach (ResultsData data in mylist)
                {
                    string value = $"Date: {data.DateRan}  Operation: {data.Operations}  Thread Count: {data.ThreadCount}  Average: {data.Average}  Standard Deviation: {data.StandardDeviation}  '\n'";
                    writefile.WriteLine(value);

                }
                Console.WriteLine($"Results sent to file: {filePath} in {Environment.CurrentDirectory}");
                createdFile = true;
                break;

            }
        }
        return filePath;
    }

    private static void sendRawDatasToFile(List<RawData> mylist)
    {
        int num = 1;
        string filePath = $"rawdata{num}.txt";
        while (!File.Exists(filePath))
        {
            num++;
            filePath = $"rawdata{num}.txt";
        }
        using StreamWriter writefile = new(filePath);
            foreach (RawData data in mylist)
            {
                string value = $"Operation: {data.Operations} Thread Count: {data.ThreadCount} Thread ID Number: {data.threadIDNumber}  Operations Count : {data.OpertionCount} '\n' ";
                writefile.WriteLine(value);

            }

        }
    
    private static void StoreData(List<ResultsData> myList, string testType, double[] avg, double[] std, string dateRan)
    {
        int[] threads = { 1, 2, 4, 8 };
        for (int i = 0; i < threads.Length; i++)
        {
            
                ResultsData result = new() { DateRan = dateRan, Operations = testType, ThreadCount = threads[i], Average = avg[i], StandardDeviation = std[i] };
                myList.Add(result);
            
        }
    }

    private static void StoreRawData(List<RawData> myList, string testType, int thread, int threadID, double operationCount)
    {

        RawData result = new() { Operations = testType, ThreadCount = thread,threadIDNumber=threadID, OpertionCount = operationCount };
        myList.Add(result);

    }

    private static void openResultsFile(string filePath )
    {
        
            try
            {   
                Process.Start("notepad.exe",filePath);

                }
            catch ( Exception e){
                Console.WriteLine($"Error occured while trying to open {filePath} reference Error Message: {e.Message} ");
            }
            }
        
    

    private static void Main()
    {
        DateTime runTimeofTest = DateTime.Now;
        string dateRan = runTimeofTest.ToString("MM_dd_yyyy_HH_mm");
        List<ResultsData> results = new();


        double[,] resultsF = new double[3, 4];
        double[,] resultsI = new double[3, 4];

        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine("Running Test #" + (i + 1));
            double[] flops = UseThreads("FLOPS");
            double[] iops = UseThreads("IOPS");
            for (int j = 0; j < 4; j++)
            {
                resultsF[i, j] = flops[j];
                resultsI[i, j] = iops[j];
            }
        }

        double[] flopsAvgs = CalculateAvg(resultsF);
        double[] iopsAvgs = CalculateAvg(resultsI);

        double[] flopsStdDev = CalculateStdDev(resultsF, flopsAvgs);
        double[] iopsStdDev = CalculateStdDev(resultsI, iopsAvgs);

       
        StoreData(results, "FLOPS", flopsAvgs, flopsStdDev, dateRan);
        StoreData(results, "Iops", iopsAvgs, iopsStdDev, dateRan);

        string filePath = sendResultsToFile(results);
        //openResultsFile(filePath);

    }
}

//Test results are in text file, need to verify results are within appropraite range. 






