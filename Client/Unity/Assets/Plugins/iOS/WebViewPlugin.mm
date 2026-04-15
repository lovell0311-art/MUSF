// Web view integration plug-in for Unity iOS.

#import <Foundation/Foundation.h>
#import <WebKit/WebKit.h>


@interface MyWebViewDelegate : NSObject<WKNavigationDelegate>

@end

@implementation MyWebViewDelegate
-(void)webView:(WKWebView *)webView decidePolicyForNavigationAction:(nonnull WKNavigationAction *)navigationAction decisionHandler:(nonnull void (^)(WKNavigationActionPolicy))decisionHandler{
//    NSString * originalUrlString = navigationAction.request.URL.absoluteString;
//    NSString * decodeUrlString = [originalUrlString stringByRemovingPercentEncoding];
//    NSURL* url = [NSURL URLWithString:decodeUrlString];
//    NSLog(@"1111111111 接收到回调。%@",url);
//    if([[url absoluteString] hasPrefix:@"weixing://wap/pay?"]){
//        [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
//        decisionHandler(WKNavigationActionPolicyCancel);
//    }else{
//        decisionHandler(WKNavigationActionPolicyAllow);
//    }
    
    // 获取原始的 URL 字符串
    NSString *originalURLString = navigationAction.request.URL.absoluteString;

    // URL 解码
    NSString *decodedURLString = [originalURLString stringByRemovingPercentEncoding];

    // 打印解码后的 URL 字符串
    NSLog(@"Decoded URL String: %@", decodedURLString);

    // 使用解码后的字符串创建新的 NSURL 对象
    NSURL *decodedURL = [NSURL URLWithString:decodedURLString];

    // 打印 NSURL 对象
    NSLog(@"Decoded NSURL: %@", decodedURL);

    // 检查并处理微信支付 URL
    if (decodedURL && [[decodedURL absoluteString] hasPrefix:@"weixin://wap/pay?"]) {
        [[UIApplication sharedApplication] openURL:decodedURL options:@{} completionHandler:nil];
        decisionHandler(WKNavigationActionPolicyCancel);  // 取消 WebView 的默认加载行为
    } else {
        decisionHandler(WKNavigationActionPolicyAllow);  // 允许 WebView 继续加载其他 URL
    }

    
}
@end


extern UIViewController *UnityGetGLViewController(); // Root view controller of Unity screen.

#pragma mark Plug-in Functions

static WKWebView *webView;
static MyWebViewDelegate *webViewDelegate;

extern "C" void _WebViewPluginInstall() {
    
    NSLog(@"1111111111 _WebViewPluginInstall 初始化");
    // Add the web view onto the root view (but don't show).
    UIViewController *rootViewController = UnityGetGLViewController();
    
    //
    webViewDelegate = [MyWebViewDelegate new];
    
    WKWebViewConfiguration *config = [[WKWebViewConfiguration alloc] init];
    webView = [[WKWebView alloc] initWithFrame:rootViewController.view.frame configuration:config];
    webView.hidden = YES;
    webView.navigationDelegate = webViewDelegate;
    
    [rootViewController.view addSubview:webView];
    
    
    
    
//    UIViewController *rootViewController = UnityGetGLViewController();
//    webView = [[UIWebView alloc] initWithFrame:rootViewController.view.frame];
//    webView.hidden = YES;
//    [rootViewController.view addSubview:webView];
}


extern "C" void _WebViewInstall() {
    
    NSLog(@"1111111111 _WebViewInstall 初始化");
    UIViewController *rootViewController = UnityGetGLViewController();
    webViewDelegate = [MyWebViewDelegate new];
    
    WKWebViewConfiguration *config = [[WKWebViewConfiguration alloc] init];
    webView = [[WKWebView alloc] initWithFrame:rootViewController.view.frame configuration:config];
    webView.navigationDelegate = webViewDelegate;
    
    [rootViewController.view addSubview:webView];
}

extern "C" void _WebViewPluginMakeTransparentBackground() {
//    [webView setBackgroundColor:[UIColor clearColor]];
//    [webView setOpaque:NO];
    webView.opaque = NO;
    webView.backgroundColor = [UIColor clearColor];
}

extern "C" void _WebViewPluginLoadUrl(const char* url, Boolean isClearCache) {
//    if (isClearCache) {
//        [[NSURLCache sharedURLCache] removeAllCachedResponses];
//    }
//    [webView loadRequest:[NSURLRequest requestWithURL:[NSURL URLWithString:[NSString stringWithUTF8String:url]]]];
    
    if(isClearCache){
        [[NSURLCache sharedURLCache] removeAllCachedResponses];
    }
    NSURL *nsUrl = [NSURL URLWithString:[NSString stringWithUTF8String:url]];
    NSURLRequest *request = [NSURLRequest requestWithURL:nsUrl];
    
    [webView loadRequest:request];
}

extern "C" void _WebViewLoadUrlReferer(const char* url, Boolean isClearCache){
    
    if(isClearCache){
        [[NSURLCache sharedURLCache] removeAllCachedResponses];
    }
    NSURL *nsUrl = [NSURL URLWithString:[NSString stringWithUTF8String:url]];
    NSLog(@"1111111111 _WebViewLoadUrlReferer 打开地址 %@",nsUrl);
    NSMutableURLRequest *request = [NSMutableURLRequest requestWithURL:nsUrl];
    [request setValue:@"http://wechat.zzws.top" forHTTPHeaderField:@"Referer"];
    
    [webView loadRequest:request];
}

extern "C" void _WebViewSetVisibility(bool visibility) {
    webView.hidden = visibility;
    //webView.hidden = !visibility;
    NSLog(@"1111111111 _WebViewSetVisibility 设置显示状态");
}

extern "C" void _WebViewPluginSetVisibility(bool visibility) {
    webView.hidden = visibility ? NO : YES;
    //webView.hidden = !visibility;
    NSLog(@"1111111111 _WebViewPluginInstall 设置显示状态");
}

extern "C" void _WebViewPluginSetMargins(int left, int top, int right, int bottom) {
//    UIViewController *rootViewController = UnityGetGLViewController();
//
//    CGRect frame = rootViewController.view.frame;
//    CGFloat scale = rootViewController.view.contentScaleFactor;
//
//    CGRect screenBound = [[UIScreen mainScreen] bounds];
//    CGSize screenSize = screenBound.size;
//    // Obtaining the current device orientation
//    UIDeviceOrientation orientation = [[UIDevice currentDevice] orientation];
//
//    // landscape
//    if (orientation) {
//        frame.size.width = screenSize.height - (left + right) / scale;
//        frame.size.height = screenSize.width - (top + bottom) / scale;
//    } else { // portrait
//        frame.size.width = screenSize.width - (left + right) / scale;
//        frame.size.height = screenSize.height - (top + bottom) / scale;
//    }
//
//    frame.origin.x += left / scale;
//    frame.origin.y += top / scale;
//
//    webView.frame = frame;
    
    UIViewController *rootViewController = UnityGetGLViewController();
    CGRect frame = rootViewController.view.frame;
    CGFloat scale = rootViewController.view.contentScaleFactor;
    CGRect screenBound = [[UIScreen mainScreen] bounds];
    CGSize screenSize = screenBound.size;
    UIDeviceOrientation orientation = [[UIDevice currentDevice] orientation];
    if(UIDeviceOrientationIsLandscape(orientation)){
        frame.size.width = screenSize.height - (left + right) / scale;
        frame.size.height = screenSize.width - (top + bottom) / scale;
    }else{
        frame.size.width = screenSize.width - (left + right) / scale;
        frame.size.height = screenSize.height - (top + bottom )/scale;
    }
    frame.origin.x += left/scale;
    frame.origin.y += top / scale;
    webView.frame = frame;
    
}

extern "C" char *_WebViewPluginPollMessage() {
    // Try to retrieve a message from the message queue in JavaScript context.
//    NSString *message = [webView stringByEvaluatingJavaScriptFromString:@"unityWebMediatorInstance.pollMessage()"];
//    if (message && message.length > 0) {
//        NSLog(@"UnityWebViewPlugin: %@", message);
//        char* memory = static_cast<char*>(malloc(strlen(message.UTF8String) + 1));
//        if (memory) strcpy(memory, message.UTF8String);
//        return memory;
//    } else {
//        return NULL;
//    }
//    __block NSString *result = nil;
//    dispatch_semaphore_t semaphore = dispatch_semaphore_create(0);
//    [webView evaluateJavaScript:[NSString stringWithUTF8String:script] completionHandler:<#^(id _Nullable, NSError * _Nullable error)completionHandler#>]
    
}

extern "C" char *_WebViewPluginCallJavascript(const char* script) {
//    NSString *message = [webView stringByEvaluatingJavaScriptFromString:[NSString stringWithUTF8String:script]];
//    if (message && message.length > 0) {
//        char* memory = static_cast<char*>(malloc(strlen(message.UTF8String) + 1));
//        if (memory) strcpy(memory, message.UTF8String);
//        return memory;
//    } else {
//        return NULL;
//    }
    __block NSString *result = nil;
    dispatch_semaphore_t semaphore = dispatch_semaphore_create(0);
    [webView evaluateJavaScript:[NSString stringWithUTF8String:script] completionHandler:^(id _Nullable response, NSError * _Nullable error) {
        if([response isKindOfClass:[NSString class]]){
            result = response;
        }
        dispatch_semaphore_signal(semaphore);
    }];
    
    dispatch_semaphore_wait(semaphore, DISPATCH_TIME_FOREVER);
    
    if(result && result.length > 0){
        char *memory = static_cast<char*>(malloc(strlen(result.UTF8String)+1));
        if(memory) strcpy(memory, result.UTF8String);
        return memory;
    }else{
        return NULL;
    }
}

extern "C" void _WebViewRemove(){
    if(webView){
        [webView removeFromSuperview];
        webView = nil;
        NSLog(@"1111111111 _WebViewRemove 移除页面");
    }
}

//extern "C" void AliPay_iOS(const char* url) {
	//NSString* orderString = [NSString stringWithUTF8String:url];
	//NSString *appScheme = @"alisdkpay";
    // NOTE: 调用支付结果开始支付
    //[[AlipaySDK defaultService] payOrder:orderString fromScheme:appScheme callback:^(NSDictionary *resultDic) {
     //   NSLog(@"reslut = %@",resultDic);
    //}];
//}

