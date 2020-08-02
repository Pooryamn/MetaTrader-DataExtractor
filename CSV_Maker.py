import glob
import os
import csv
from time import sleep
from zipfile import ZipFile
import datetime
import shutil

os.chdir(os.getcwd())


directory = "Data"
  
# Parent Directory path 
parent_dir = os.getcwd()
  
# Path of files 
path = os.path.join(parent_dir, directory) 
  
# Create the directory 
try: 
    os.makedirs(path) 
except OSError as error: 
    pass 

try:
    while (1):

        sleep(0.5)

        Day = datetime.datetime.now().weekday()
        Hour =  int(str(datetime.datetime.now().time()).split(':')[0])
        if(Day == 3): # it  is Thursday
            print('Market is Closed !')
            print('Weekday : {}'.format(Day))
            sleep(86400) # One day
            continue

        if(Day == 4): # Friday
            print('Market is Closed !')

            if(Hour <= 12):    
                print('Weekday : {}'.format(Day))
                sleep(43200) # 12 Hours   
                continue
            else:
                print('Time : {}'.format(str(datetime.datetime.now().time())))
                sleep(28800) # 8 Hours
                continue

        
        if(Hour >= 13 or Hour < 8):
            print('Market is Closed !')
            if(Hour < 7):
                print('Time : {}'.format(str(datetime.datetime.now().time())))
                sleep(3600) # 1 Hour
                continue
            if(Hour == 7):
                print('Time : {}'.format(str(datetime.datetime.now().time())))
                sleep(900) # 15 minute
                continue
            if(Hour >= 13):
                print('Time : {}'.format(str(datetime.datetime.now().time())))
                sleep(43200) # 12 Houres
                continue
            
        
        # CSV File
        datafile = open('Data\DATA_FILE.csv','w',newline='')


        data_writer = csv.writer(datafile,delimiter=',',quotechar='"',quoting=csv.QUOTE_MINIMAL)

        #Csv Headers :
        header = ['Date','Time','BID','ASK','Last','Last High','Last Low','Change','Deals','Deals Volume','Open','Close']
        data_writer.writerow(header)

        #Symbol File :
        SYM = open('Data\Symbols.txt','w',encoding='utf-8')

        FileList = []

        # read all txt files for get name :
        for file in glob.glob("*.txt"):
            FileList.append(file)
            Name = file.split('M1')[0]
            SYM.write(Name)
            SYM.write('\n')
    
        SYM.close()

        Counter = 0
        L = ""
        
        # Clock 
        Sys_Hour = datetime.datetime.now().hour
        Sys_Min = datetime.datetime.now().minute

        Sys_Time = str(Sys_Hour) + ':' + str(Sys_Min)

        # write data to files :
        for file in FileList:
            try:
                with open(file,'r',encoding='utf-16') as fp:
                    L = fp.readline()
                    row = L.split(",")
                    data_writer.writerow([row[0],Sys_Time,row[2],row[3],row[4],row[5],row[6],row[7],row[8],row[9],row[10],row[11]])
                    fp.close()
                    Counter += 1
            except:
                try:
                    sleep(0.2)
                    with open(file,'r',encoding='utf-16') as fp:
                        L = fp.readline()
                        row = L.split(",")
                        data_writer.writerow([row[0],Sys_Time,row[2],row[3],row[4],row[5],row[6],row[7],row[8],row[9],row[10],row[11]])
                        fp.close()
                        Counter += 1
                except:
                    continue     

        datafile.close()

        print(str(Counter) + " rows effected !" )

        # write to zip
        ZipObj = ZipFile('Data.zip','w')

        
        ZipObj.write("Data\DATA_FILE.csv")
        ZipObj.write("Data\Symbols.txt")

        ZipObj.close()

        ## rename files for web users :

        dst_name ='Report_'+str(datetime.datetime.now().date())+'_' + str(datetime.datetime.now().hour)+'-'+str(datetime.datetime.now().minute)+'-' + str(datetime.datetime.now().second)
        new_csv_name = dst_name+'.csv'
        new_txt_name = dst_name+'.txt'
        #new_csv = os.path.join(path,new_csv_name)
        #new_txt = os.path.join(path,new_txt_name) 
        old_csv = os.path.join(path,'DATA_FILE.csv')
        old_txt = os.path.join(path,'Symbols.txt')
        
        # cut data from program folder to desktop for web use :
        dst_path = 'C:\\Users\\Administrator\\Desktop\\Project\\DATA'

        try: 
            os.makedirs(dst_path) 
        except OSError as error: 
            pass

        new_csv = os.path.join(dst_path,new_csv_name)
        new_txt = os.path.join(dst_path,new_txt_name)

        shutil.move(old_csv,new_csv)
        shutil.move(old_txt,new_txt)


# if program crashed -> run it again :))
except :
    runner_path = parent_dir+'\CSV_Maker.exe'
    os.system(runner_path)
##########
      
