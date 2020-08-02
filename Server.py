import socket
from threading import Thread
from socketserver import ThreadingMixIn
import glob
import os
import time
from zipfile import ZipFile


TCP_PORT = 12001 # this port was opened in firewall
BUFFER_SIZE = 1024


class ClientThread(Thread):

    # make thread for client
    def __init__(self,ip,port,sock):
        Thread.__init__(self)
        self.ip = ip
        self.port = port
        self.sock = sock
        print (" New thread started for "+ip+":"+str(port))

    # send file for client
    def run(self):

        fileList = []

        filename=bytes("Data.zip",'utf-8')
            
        f = open(filename,'rb')

        self.sock.send(filename)

        time.sleep(0.5)

        while True:
            l = f.read(BUFFER_SIZE)
            while (l):
                self.sock.send(l)
                l = f.read(BUFFER_SIZE)
            if not l:
                f.close()
                self.sock.close()
                break

tcpsock = socket.socket(socket.AF_INET, socket.SOCK_STREAM) #TCP Connection
tcpsock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
tcpsock.bind(("",TCP_PORT))
threads = []

while True:
    tcpsock.listen(5)
    print ("Waiting for incoming connections...")
    (conn, (ip,port)) = tcpsock.accept()
    print ('Got connection from ', (ip,port))
    newthread = ClientThread(ip,port,conn)
    newthread.start()
    threads.append(newthread)

for t in threads:
    t.join()
