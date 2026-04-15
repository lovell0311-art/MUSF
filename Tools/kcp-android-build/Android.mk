LOCAL_PATH := $(call my-dir)

include $(CLEAR_VARS)

LOCAL_MODULE := kcp
LOCAL_SRC_FILES := kcp_wrap.c

include $(BUILD_SHARED_LIBRARY)
