import socket
import os
from zipfile import ZipFile
import sys
from time import sleep

try:
    TCP_IP = 'localhost'
#TCP_IP = '192.168.1.103'
    TCP_PORT = 12001
    BUFFER_SIZE = 1024

    s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

    try:
        s.connect((TCP_IP, TCP_PORT))
    except :
        sys.exit(0)
###############

    fname = s.recv(BUFFER_SIZE)
    filename = fname.decode("utf-8")
    print(filename+"\n")

##############


    with open(filename,'wb') as f:
        print ('file opened')
        while True:
            #print('receiving data...')
            data = s.recv(BUFFER_SIZE)
            #print('data=%s', (data))
            if not data:
                f.close()
                print ('file close()')
                break
            # write data to a file
            f.write(data)

    print('Successfully get the file')
    s.close()
    print('connection closed')

    with ZipFile(filename, 'r') as zip: 
        # extracting all the files 
        print('Extracting all the files now...') 
        zip.extractall() 
        print('Done!')

    os.remove("Data.zip")

except :
    sys.exit(0)



