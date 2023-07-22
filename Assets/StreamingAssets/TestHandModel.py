
import cv2
from cvzone.HandTrackingModule import HandDetector
from cvzone.ClassificationModule import Classifier
import numpy as np
import math
import time
cap = cv2.VideoCapture(0)
detector = HandDetector(maxHands=1)
classifier = Classifier("Model/keras_model", "Model/labels")

offset = 20
imgSize = 300
pTime=0
#folder = "Data/C"
counter = 0

labels = ["OK", "NO",]

while True:
    success, img = cap.read()
    imgOutput = img.copy()
    hands, img = detector.findHands(img)
    if hands:
        hand = hands[0]
        x, y, w, h = hand['bbox']

        imgWhite = np.ones((imgSize, imgSize, 3), np.uint8) * 255
        imgCrop = img[y - offset:y + h + offset, x - offset:x + w + offset]

        imgCropShape = imgCrop.shape




        # cv2.putText(flip_img, f'FPS: {int(fps)}', (40, 50), cv2.FONT_HERSHEY_COMPLEX,
        #try:
         #   cv2.imshow("ImageCrop", imgCrop)
          #  cv2.imshow("ImageWhite", imgWhite)
        #except Exception as e:
         #   print(str(e))
    cv2.imshow("Image", imgOutput)
    cv2.waitKey(1)