//
//  RemoteCentral.h
//  RemoteCentral
//
//  Created by John Murray on 11/2/13.
//  Copyright (c) 2013 Seebright, Inc. All rights reserved.
//

//#import <opencv2/opencv.hpp>

#import <Foundation/Foundation.h>
#import <CoreBluetooth/CoreBluetooth.h>
//#include <opencv2/highgui/cap_ios.h>
//#include <opencv2/highgui/highgui.hpp>
//#include <opencv2/imgproc/imgproc.hpp>


@interface RemoteCentral : NSObject <CBCentralManagerDelegate, CBPeripheralDelegate/*, CvVideoCameraDelegate*/>
//{
//    // OpenCV Video Camera
//    CvVideoCamera* videoCamera;
//}
-(void)initOccurred;
// Bluetooth Properties
@property (nonatomic, strong) CBPeripheral  *discoveredPeripheral;
@property (nonatomic, strong) NSMutableData *data;
@property (nonatomic, strong) CBCentralManager *manager;

// Remote Joystick Properties
@property ( atomic, readwrite ) short joy_x;
@property ( atomic, readwrite ) short joy_y;

// Remote Button Properties
@property ( atomic, readwrite ) bool bSelect;
@property ( atomic, readwrite ) bool bBack;
@property ( atomic, readwrite ) bool bOption;
@property ( atomic, readwrite ) bool bUp;
@property ( atomic, readwrite ) bool bDown;
@property ( atomic, readwrite ) bool bTrigger;
@property ( atomic, readwrite ) bool bNav;

// Remote Quaternian Properties
@property ( atomic, readwrite ) short quat_x;
@property ( atomic, readwrite ) short quat_y;
@property ( atomic, readwrite ) short quat_z;
@property ( atomic, readwrite ) short quat_w;

// Remote Position
@property ( atomic, readwrite ) short pos_x;
@property ( atomic, readwrite ) short pos_y;
@property ( atomic, readwrite ) short radius;

// Remote data output
@property ( atomic, readwrite ) char* dataChar;

// OpenCV Camera input
//@property (nonatomic, retain) CvVideoCamera* videoCamera;

@end
