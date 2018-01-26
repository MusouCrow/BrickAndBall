import socket

lst = []
sk = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
sk.bind(('localhost', 12345))
print("server started")

while True:
    data, address = sk.recvfrom(1024)
    lst.append(address)
    print("add " + str(address))

    if len(lst) > 1:
        a = lst.pop()
        b = lst.pop()
        sk.sendto(str(a).encode("ascii"), a)
        sk.sendto(str(b).encode("ascii"), b)
        print("exchange " + str(a) + " and " + str(b))

sk.close()
