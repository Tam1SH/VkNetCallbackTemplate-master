import clr
import sys
import System
from System import Action
def CallBack(__x__,__y__):
    try:
        exec(__y__)
        __x__.Invoke(result.ToString())
    except Exception as ex:
        if ex.message == "name 'result' is not defined":
            __x__.Invoke("Python: expression must have 'result'")
        else: 
            __x__.Invoke("Python: " + ex.message)

CallBack(act,text)  
