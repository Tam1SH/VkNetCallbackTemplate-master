import clr
import sys
import System
from System import Action
import vk
import random
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


def PIZDA(text,pizdec):
    try:
        pizda  = urllib.request.urlopen('https://narfu.ru/sf/stc/forstud/rasp/15.02.21-O.pdf').read()
        f = open("/app/VkBot/infile.pdf", "wb")
        f.write(pizda)
        f.close()

        pizdec.Invoke()

        group_id = '200286651'
        access_token = '8195f5251b143964f6085ddff9b4251e9b2cdf33c95ca17d6c8681bf98688f26af13dc4b9d3d71443d1e3'

        vk.VERSION = '5.50'
        session = vk.Session(access_token=access_token)
        vk_api = vk.API(session)


        upload_url = vk_api.photos.getMessagesUploadServer(group_id=group_id, v = '5.50')['upload_url']

        request = requests.post(upload_url, files={'photo': open("/app/VkBot/outfile.png", "rb")})
        params = {'server': request.json()['server'],
                  'photo': request.json()['photo'],
                  'hash': request.json()['hash'],
                  'group_id': group_id,
                  'v': '5.50' }

        photo_id = vk_api.photos.saveMessagesPhoto(**params)[0]['id']
        params = {'peer_id': 2000000119,
                  'random_id': random.randint(0,100000000),
                  'attachment': 'photo'+'-'+str(group_id)+'_'+str(photo_id),
                  'message': 'message',
                  'v': '5.50' }
        vk_api.messages.send(**params)
        pizdec.Invoke(photo_id)
    except Exception as ex:
        pizdec.Invoke(ex.message)


PIZDA(text,pizdec)