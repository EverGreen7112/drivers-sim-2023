import struct
import socket
import keyboard


# this code allows you to control the robot in the simulation using your keyboard
# this code still sends data over LAN 
port = 5804 

xyz = [0.0, 0.0, 0.0]
angle = [0.0, 0.0, 0.0]
move_speed = 0.01
turn_speed = 0.5
while True:
    # location
    if keyboard.is_pressed("d"):
        xyz[0] += move_speed
    if keyboard.is_pressed("a"):
        xyz[0] -= move_speed
    if keyboard.is_pressed("s"):
        xyz[2] -= move_speed
    if keyboard.is_pressed("w"):
        xyz[2] += move_speed
        
    #angle
    if keyboard.is_pressed("right"):
        angle[1] += turn_speed
    if keyboard.is_pressed("left"):
        angle[1] -= turn_speed
    if keyboard.is_pressed("down"):
        angle[2] -= turn_speed
    if keyboard.is_pressed("up"):
        angle[2] += turn_speed
    print(xyz)
    # print(angle)
    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM, socket.IPPROTO_UDP) as sock:
            sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
            pack = struct.pack('ffffff', xyz[0], xyz[1], xyz[2], 
                                    angle[0], angle[1], angle[2])
            sock.sendto(pack, ("255.255.255.255", port))