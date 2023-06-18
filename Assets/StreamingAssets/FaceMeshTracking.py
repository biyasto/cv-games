import imutils
import mediapipe as mp
import cv2
import time
import socket

from imutils.video import WebcamVideoStream

width, height = 1280,720

#cap = cv2.VideoCapture(0)
#cap.set(3,width)
#cap.set(4,height)
#cap = cv2.VideoCapture(0)

cap = WebcamVideoStream(src=0).start()



# Checking Camera is Opened or Not

facmesh = mp.solutions.face_mesh
face = facmesh.FaceMesh(static_image_mode=True, min_tracking_confidence=0.6, min_detection_confidence=0.6)
draw = mp.solutions.drawing_utils


#unity communicate
sock = socket.socket(socket.AF_INET,socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

pTime=0
while True:

	#_, frm = cap.read()

	frm = cap.read()  # reading Frame
	frm = imutils.resize(frm, width=400)
	#print(frm.shape)

	rgb = cv2.cvtColor(frm, cv2.COLOR_BGR2RGB)

	op = face.process(rgb)
	if op.multi_face_landmarks:
		for i in op.multi_face_landmarks:
			#print(i.landmark[8])
			draw.draw_landmarks(frm, i, facmesh.FACEMESH_CONTOURS, landmark_drawing_spec=draw.DrawingSpec(color=(0, 255, 255), circle_radius=1))

			data = []
			data.extend([i.landmark[8].x,i.landmark[8].y])
			data.extend([i.landmark[119].x, i.landmark[119].y])
			data.extend([i.landmark[148].x, i.landmark[148].y])
			#print(data)
			sock.sendto(str.encode(str(data)), serverAddressPort)

	cTime = time.time()
	fps = 1 / (cTime - pTime)
	pTime = cTime
	#cv2.putText(frm, f'FPS: {int(fps)}', (40, 50), cv2.FONT_HERSHEY_COMPLEX,
	#			1, (255, 0, 0), 3)

	#cv2.imshow("window", frm)


	if cv2.waitKey(1) == 27:
		cap.release()
		cv2.destroyAllWindows()



