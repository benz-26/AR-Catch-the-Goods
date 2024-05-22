import cv2
import numpy as np
import socket

# Parameters
width, height = 1280, 720

# Initialize webcam
cap = cv2.VideoCapture(1)
cap.set(3, width)
cap.set(4, height)

# Load YOLOv4-tiny
net = cv2.dnn.readNet("yolov4-tiny.weights", "yolov4-tiny.cfg")
layer_names = net.getLayerNames()
output_layers = [layer_names[i - 1] for i in net.getUnconnectedOutLayers()]

# UDP Socket
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
serverAddressPort = ("127.0.0.1", 5052)

# Initialize variables for tracking horizontal movement
prev_x = None

while True:
    # Get the frame from the webcam
    success, img = cap.read()
    if not success:
        break

    # Prepare the image for YOLO
    blob = cv2.dnn.blobFromImage(img, 0.00392, (416, 416), (0, 0, 0), True, crop=False)
    net.setInput(blob)
    outs = net.forward(output_layers)

    # Analyze the detections
    class_ids = []
    confidences = []
    boxes = []

    for out in outs:
        for detection in out:
            scores = detection[5:]
            class_id = np.argmax(scores)
            confidence = scores[class_id]
            if confidence > 0.5 and class_id == 0:  # Assuming class_id 0 is 'person'
                center_x = int(detection[0] * width)
                center_y = int(detection[1] * height)
                w = int(detection[2] * width)
                h = int(detection[3] * height)

                x = int(center_x - w / 2)
                y = int(center_y - h / 2)

                boxes.append([x, y, w, h])
                confidences.append(float(confidence))
                class_ids.append(class_id)

    indexes = cv2.dnn.NMSBoxes(boxes, confidences, 0.5, 0.4)

    data = []

    if len(indexes) > 0:
        i = indexes[0]
        box = boxes[i]
        x, y, w, h = box
        current_x = x + w // 2  # Center x-coordinate of the bounding box

        if prev_x is not None:
            # Calculate horizontal movement
            horizontal_movement = current_x - prev_x
            data.append(horizontal_movement)
            # Send data to Unity
            sock.sendto(str.encode(str(data)), serverAddressPort)

        prev_x = current_x

        # Draw bounding box around the detected person
        cv2.rectangle(img, (x, y), (x + w, y + h), (0, 255, 0), 2)

    # Resize and display the image
    img = cv2.resize(img, (0, 0), None, 0.5, 0.5)
    cv2.imshow("Image", img)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
