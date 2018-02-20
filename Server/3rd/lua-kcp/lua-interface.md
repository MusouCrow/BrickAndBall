
# LUA-INTERFACE

## LKcp.lkcp_create(session, func)

### DESCRIPTION
    Create kcp object.

### PARAMETERS
    session: number mark session 
    func: extra closures, which KCP layer invoke callback to send data, see detail in testkcp.lua

### RETURN
    kcp: kcp object

## kcp:lkcp_wndsize(sndwnd, rcvwnd)

### DESCRIPTION
    Set maximum window size: sndwnd=32, rcvwnd=32 by default

### PARAMETERS
    sndwnd: send window size
    rcvwnd: recive window size

### RETURN
    None

## kcp:lkcp_nodelay(nodelay, interval, resend, nc)

### DESCRIPTION
    Config re-transmission and flow control

### PARAMETERS
    nodelay: 0:disable(default), 1:enable
    interval: internal update timer interval in millisec, default is 100ms 
    resend: 0:disable fast resend(default), 1:enable fast resend
    nc: 0:normal congestion control(default), 1:disable congestion control

### RETURN
    ret: always 0

## kcp:lkcp_check(current)

### DESCRIPTION
    Get when to invoke lkcp_update

### PARAMETERS
    current: current timestamp in millisec

### RETURN
    when: timestamp in millisec when to invoke lkcp_update 

## kcp:lkcp_update(current)

### DESCRIPTION
    Update state (call it repeatedly, every 10ms-100ms), or you can ask 

### PARAMETERS
    current: current timestamp in millisec

### RETURN
    None

## kcp:lkcp_send(data)

### DESCRIPTION
    User/upper level send

### PARAMETERS
    data: data to be sent

### RETURN
    sent_len: below zero for error, otherwise succeed

## kcp:lkcp_flush()

### DESCRIPTION
    Flush pending data

### PARAMETERS
    None

### RETURN
    None

## kcp:lkcp_input(data)

### DESCRIPTION
    When you received a low level packet (eg. UDP packet), call it

### PARAMETERS
    data: data received from transport layer

### RETURN
    ret: below zero for error, otherwise succeed 

## kcp:lkcp_recv()

### DESCRIPTION
    User/upper level recv 

### PARAMETERS
    None

### RETURN
    rcv_len: Less than or equal to 0 for EAGAIN, otherwise for rcv_len 
    rcv_buf: if rcv_len greater than 0, rcv_buf is data to recv

