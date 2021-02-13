import clr
import sys
import System
from System import Action
import fitz
from datetime import datetime, date, time
import requests
import urllib
def CallBack(__x__,__y__):
    try:
        exec(__y__)
        __x__.Invoke(result.ToString())
    except Exception as ex:
        if ex.message == "name 'result' is not defined":
            __x__.Invoke("Python: expression must have 'result'")
        else: 
            __x__.Invoke("Python: " + ex.message)
def Pizda(__x):
    try:
        pizda  = urllib.request.urlopen('https://narfu.ru/sf/stc/forstud/rasp/10.02.21-O.pdf').read()
        f = open("/app/VkBot/file.pdf", "wb")
        f.write(pizda)
        f.close()
        pdffile = "/app/VkBot/file.pdf"
        doc = fitz.open(pdffile)
        page = doc.loadPage(0)  # number of page
        pix = page.getPixmap()
        output = "/app/VkBot/outfile.png"
        pix.writePNG(output)
    except Exception as ex:
        print("pizda nakrilas: "  + ex.message)

if act != None:
    CallBack(act,text) 
if text != None:
    Pizda(text)