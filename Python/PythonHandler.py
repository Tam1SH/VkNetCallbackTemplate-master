import clr
import sys
import System
from System import Action
from pdf2image import convert_from_path
def CallBack(__x__,__y__):
    try:
        exec(__y__)
        __x__.Invoke(result.ToString())
    except Exception as ex:
        if ex.message == "name 'result' is not defined":
            __x__.Invoke("Python: expression must have 'result'")
        else: 
            __x__.Invoke("Python: " + ex.message)

def PdfToImage(__x__):
    pages = convert_from_path('pdf_file', 500)
    for page in pages:
        page.save('out.jpg', 'JPEG')
if act != None:
    CallBack(act,text)  


