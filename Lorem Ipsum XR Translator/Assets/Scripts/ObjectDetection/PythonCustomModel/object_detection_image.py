import cv2
import json

image_path = ''
image = cv2.imread(image_path)

#image = cv2.imread('examples/image4.jpg')
#image_path = 'examples/desk.jpeg'
#image = cv2.imread(image_path)

image = cv2.resize(image, (640, 480))
h = image.shape[0]
w = image.shape[1]

response_dict = {}
response_dict["requestId"] = "N/A"
response_dict["modelVersion"] = "N/A"
meta_dict = {
        "height":h,
        "width":w,
        "format":"Jpeg"
    }
response_dict["metadata"] = meta_dict

object_list = []

# path to the weights and model files
weights = "ssd_mobilenet/frozen_inference_graph.pb"
model = "ssd_mobilenet/ssd_mobilenet_v3_large_coco_2020_01_14.pbtxt"
# load the MobileNet SSD model trained  on the COCO dataset
net = cv2.dnn.readNetFromTensorflow(weights, model)

# load the class labels the model was trained on
class_names = []
with open("ssd_mobilenet/coco_names.txt", "r") as f:
    class_names = f.read().strip().split("\n")

# create a blob from the image
blob = cv2.dnn.blobFromImage(
    image, 1.0/127.5, (320, 320), [127.5, 127.5, 127.5])
# pass the blog through our network and get the output predictions
net.setInput(blob)
output = net.forward()  # shape: (1, 1, 100, 7)

# loop over the number of detected objects
for detection in output[0, 0, :, :]:  # output[0, 0, :, :] has a shape of: (100, 7)
    obj_dict = {}
    # the confidence of the model regarding the detected object
    probability = detection[2]
    obj_dict["confidence"] = float(detection[2])

    # if the confidence of the model is lower than 50%,
    # we do nothing (continue looping)
    if probability < 0.5:
        continue
    #print(detection)
    # perform element-wise multiplication to get
    # the (x, y) coordinates of the bounding box
    box = [int(a * b) for a, b in zip(detection[3:7], [w, h, w, h])]
    box = tuple(box)
    #print(box)
    # draw the bounding box of the object
    cv2.rectangle(image, box[:2], box[2:], (0, 255, 0), thickness=2)
    rect_dict = {
            "x": box[0],
            "y": box[2],
            "w": box[2]-box[0],
            "h": box[3]-box[1],
        }
    # extract the ID of the detected object to get its name
    class_id = int(detection[1])
    # draw the name of the predicted object along with the probability
    label = f"{class_names[class_id - 1].upper()} {probability * 100:.2f}%"
    obj_dict["object"] = class_names[class_id - 1]
    cv2.putText(image, label, (box[0], box[1] + 15),
                cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
    obj_dict["rectangle"] = rect_dict
    object_list.append(obj_dict)

response_dict["objects"] = object_list

response_text = json.dumps(response_dict)
print(response_text)

#cv2.imshow('Image', image)
#cv2.waitKey()
