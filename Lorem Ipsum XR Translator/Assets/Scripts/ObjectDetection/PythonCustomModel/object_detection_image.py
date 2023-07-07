import cv2
import json

image_path = ''
if image_path != '':
    image = cv2.imread(image_path)
    
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
    
    weights = "ssd_mobilenet/frozen_inference_graph.pb"
    model = "ssd_mobilenet/ssd_mobilenet_v3_large_coco_2020_01_14.pbtxt"
    
    net = cv2.dnn.readNetFromTensorflow(weights, model)
    
    class_names = []
    with open("ssd_mobilenet/coco_names.txt", "r") as f:
        class_names = f.read().strip().split("\n")
    
    blob = cv2.dnn.blobFromImage(
        image, 1.0/127.5, (320, 320), [127.5, 127.5, 127.5])
    
    net.setInput(blob)
    output = net.forward()  # shape: (1, 1, 100, 7)
    
    for detection in output[0, 0, :, :]:  # output[0, 0, :, :] has a shape of: (100, 7)
        obj_dict = {}
        
        probability = detection[2]
        obj_dict["confidence"] = float(detection[2])
    
        if probability < 0.5:
            continue
        
        box = [int(a * b) for a, b in zip(detection[3:7], [w, h, w, h])]
        box = tuple(box)
        
        cv2.rectangle(image, box[:2], box[2:], (0, 255, 0), thickness=2)
        rect_dict = {
                "x": box[0],
                "y": box[2],
                "w": box[2]-box[0],
                "h": box[3]-box[1],
            }
        
        class_id = int(detection[1])
        
        label = f"{class_names[class_id - 1].upper()} {probability * 100:.2f}%"
        obj_dict["object"] = class_names[class_id - 1]
        cv2.putText(image, label, (box[0], box[1] + 15),
                    cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 2)
        obj_dict["rectangle"] = rect_dict
        object_list.append(obj_dict)
    
    response_dict["objects"] = object_list
    
    response_text = json.dumps(response_dict)
