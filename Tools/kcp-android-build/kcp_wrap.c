#include <stdlib.h>
#include <string.h>

#define ikcp_create ikcp_create_raw
#define ikcp_release ikcp_release_raw
#define ikcp_setoutput ikcp_setoutput_raw
#define ikcp_recv ikcp_recv_raw
#define ikcp_send ikcp_send_raw
#define ikcp_update ikcp_update_raw
#define ikcp_check ikcp_check_raw
#define ikcp_input ikcp_input_raw
#define ikcp_flush ikcp_flush_raw
#define ikcp_peeksize ikcp_peeksize_raw
#define ikcp_setmtu ikcp_setmtu_raw
#define ikcp_wndsize ikcp_wndsize_raw
#define ikcp_waitsnd ikcp_waitsnd_raw
#define ikcp_nodelay ikcp_nodelay_raw
#define ikcp_log ikcp_log_raw
#define ikcp_allocator ikcp_allocator_raw
#define ikcp_getconv ikcp_getconv_raw

#include "ikcp.c"

#undef ikcp_create
#undef ikcp_release
#undef ikcp_setoutput
#undef ikcp_recv
#undef ikcp_send
#undef ikcp_update
#undef ikcp_check
#undef ikcp_input
#undef ikcp_flush
#undef ikcp_peeksize
#undef ikcp_setmtu
#undef ikcp_wndsize
#undef ikcp_waitsnd
#undef ikcp_nodelay
#undef ikcp_log
#undef ikcp_allocator
#undef ikcp_getconv

ikcpcb* ikcp_create(IUINT32 conv, void *user)
{
    return ikcp_create_raw(conv, user);
}

void ikcp_release(ikcpcb *kcp)
{
    ikcp_release_raw(kcp);
}

void ikcp_setoutput(ikcpcb *kcp, int (*output)(const char *buf, int len, ikcpcb *kcp, void *user))
{
    ikcp_setoutput_raw(kcp, output);
}

int ikcp_recv(ikcpcb *kcp, char *buffer, int len)
{
    return ikcp_recv_raw(kcp, buffer, len);
}

int ikcp_send(ikcpcb *kcp, const char *buffer, int len)
{
    return ikcp_send_raw(kcp, buffer, len);
}

void ikcp_update(ikcpcb *kcp, IUINT32 current)
{
    ikcp_update_raw(kcp, current);
}

IUINT32 ikcp_check(const ikcpcb *kcp, IUINT32 current)
{
    return ikcp_check_raw(kcp, current);
}

static IUINT32 kcp_wrap_read_u32(const char *buffer)
{
    return ((IUINT32)(unsigned char)buffer[0]) |
           ((IUINT32)(unsigned char)buffer[1] << 8) |
           ((IUINT32)(unsigned char)buffer[2] << 16) |
           ((IUINT32)(unsigned char)buffer[3] << 24);
}

static void kcp_wrap_rewrite_conv(char *buffer, int size, IUINT32 conv)
{
    int offset = 0;
    while (offset + IKCP_OVERHEAD <= size)
    {
        memcpy(buffer + offset, &conv, sizeof(IUINT32));

        IUINT32 len = kcp_wrap_read_u32(buffer + offset + 20);
        int next = offset + IKCP_OVERHEAD + (int)len;
        if (next <= offset || next > size)
        {
            return;
        }

        offset = next;
    }
}

int ikcp_input(ikcpcb *kcp, const char *data, int offset, int size)
{
    if (kcp == NULL || data == NULL || size < 0 || offset < 0)
    {
        return -1;
    }

    const char *payload = data + offset;
    if (size < (int)IKCP_OVERHEAD)
    {
        return ikcp_input_raw(kcp, payload, size);
    }

    char *buffer = (char *)malloc((size_t)size);
    if (buffer == NULL)
    {
        return -2;
    }

    memcpy(buffer, payload, (size_t)size);
    kcp_wrap_rewrite_conv(buffer, size, kcp->conv);

    int ret = ikcp_input_raw(kcp, buffer, size);
    free(buffer);
    return ret;
}

void ikcp_flush(ikcpcb *kcp)
{
    ikcp_flush_raw(kcp);
}

int ikcp_peeksize(const ikcpcb *kcp)
{
    return ikcp_peeksize_raw(kcp);
}

int ikcp_setmtu(ikcpcb *kcp, int mtu)
{
    return ikcp_setmtu_raw(kcp, mtu);
}

int ikcp_wndsize(ikcpcb *kcp, int sndwnd, int rcvwnd)
{
    return ikcp_wndsize_raw(kcp, sndwnd, rcvwnd);
}

int ikcp_waitsnd(const ikcpcb *kcp)
{
    return ikcp_waitsnd_raw(kcp);
}

int ikcp_nodelay(ikcpcb *kcp, int nodelay, int interval, int resend, int nc)
{
    return ikcp_nodelay_raw(kcp, nodelay, interval, resend, nc);
}

void ikcp_setminrto(ikcpcb *kcp, int minrto)
{
    if (kcp == NULL)
    {
        return;
    }

    kcp->rx_minrto = minrto;
}

IUINT32 ikcp_getconv(const void *ptr)
{
    if (ptr == NULL)
    {
        return 0;
    }

    return ((const ikcpcb *)ptr)->conv;
}
