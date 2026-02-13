#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>
#import <UserMessagingPlatform/UserMessagingPlatform.h>
#include <stdlib.h>

extern "C" {
    void UnitySendMessage(const char* obj, const char* method, const char* msg);
    UIViewController* UnityGetGLViewController();
}

static const char* kCallbackObject = "__EasyUmpCallbacks";

static const char* kOnInitSuccess = "OnInitSuccess";
static const char* kOnInitFailure = "OnInitFailure";
static const char* kOnShowDismissed = "OnShowDismissed";
static const char* kOnShowFailed = "OnShowFailed";
static const char* kOnReshowDismissed = "OnReshowDismissed";
static const char* kOnReshowFailed = "OnReshowFailed";

static NSString* const kJsonCode = @"Code";
static NSString* const kJsonMessage = @"Message";
static NSString* const kJsonDomain = @"Domain";
static NSString* const kJsonTagUnderAge = @"TagForUnderAgeOfConsent";
static NSString* const kJsonDebugGeography = @"DebugGeography";
static NSString* const kJsonTestDeviceIds = @"TestDeviceHashedIds";

static NSString* const kIabTcString = @"IABTCF_TCString";
static NSString* const kIabAddtlConsent = @"IABTCF_AddtlConsent";
static NSString* const kIabPurposeConsents = @"IABTCF_PurposeConsents";
static NSString* const kIabGdprApplies = @"IABTCF_gdprApplies";

static void SendMessage(const char* method, NSString* payload) {
    if (payload == nil) {
        payload = @"";
    }
    UnitySendMessage(kCallbackObject, method, payload.UTF8String);
}

static void SendError(const char* method, NSInteger code, NSString* message, NSString* domain) {
    NSDictionary* dict = @{
        kJsonCode: @(code),
        kJsonMessage: message ?: @"",
        kJsonDomain: domain ?: @""
    };
    NSError* error = nil;
    NSData* data = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
    if (data == nil || error != nil) {
        NSString* safeMessage = message ?: @"";
        NSString* safeDomain = domain ?: @"";
        safeMessage = [[safeMessage stringByReplacingOccurrencesOfString:@"\\" withString:@"\\\\"] stringByReplacingOccurrencesOfString:@"\"" withString:@"\\\""];
        safeDomain = [[safeDomain stringByReplacingOccurrencesOfString:@"\\" withString:@"\\\\"] stringByReplacingOccurrencesOfString:@"\"" withString:@"\\\""];
        NSString* fallback = [NSString stringWithFormat:@"{\"Code\":%ld,\"Message\":\"%@\",\"Domain\":\"%@\"}",
                              (long)code,
                              safeMessage,
                              safeDomain];
        SendMessage(method, fallback);
        return;
    }
    NSString* json = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];
    SendMessage(method, json);
}

static UMPRequestParameters* BuildRequestParameters(NSString* optionsJson) {
    UMPRequestParameters* params = [[UMPRequestParameters alloc] init];
    if (optionsJson == nil || optionsJson.length == 0) {
        return params;
    }

    NSData* data = [optionsJson dataUsingEncoding:NSUTF8StringEncoding];
    if (data == nil) {
        return params;
    }

    NSError* error = nil;
    NSDictionary* obj = [NSJSONSerialization JSONObjectWithData:data options:0 error:&error];
    if (![obj isKindOfClass:[NSDictionary class]] || error != nil) {
        return params;
    }

    NSNumber* tagUnderAge = obj[kJsonTagUnderAge];
    if ([tagUnderAge isKindOfClass:[NSNumber class]] && tagUnderAge.boolValue) {
        params.tagForUnderAgeOfConsent = YES;
    }

    NSNumber* debugGeo = obj[kJsonDebugGeography];
    NSArray* testIds = obj[kJsonTestDeviceIds];
    if (debugGeo != nil || (testIds != nil && testIds.count > 0)) {
        UMPDebugSettings* debugSettings = [[UMPDebugSettings alloc] init];
        if ([debugGeo isKindOfClass:[NSNumber class]]) {
            debugSettings.geography = (UMPDebugGeography)debugGeo.integerValue;
        }
        if ([testIds isKindOfClass:[NSArray class]]) {
            debugSettings.testDeviceIdentifiers = testIds;
        }
        params.debugSettings = debugSettings;
    }

    return params;
}

extern "C" void EasyUmpIosBridgeInit(const char* optionsJson) {
    NSString* json = optionsJson ? [NSString stringWithUTF8String:optionsJson] : @"";
    dispatch_async(dispatch_get_main_queue(), ^{
        UMPRequestParameters* params = BuildRequestParameters(json);
        [[UMPConsentInformation sharedInstance] requestConsentInfoUpdateWithParameters:params
                                                                     completionHandler:^(NSError * _Nullable error) {
            if (error != nil) {
                SendError(kOnInitFailure, error.code, error.localizedDescription, error.domain);
            } else {
                SendMessage(kOnInitSuccess, @"{}");
            }
        }];
    });
}

extern "C" void EasyUmpIosBridgeShow() {
    dispatch_async(dispatch_get_main_queue(), ^{
        UIViewController* vc = UnityGetGLViewController();
        if (vc == nil) {
            SendError(kOnShowFailed, -1, @"Unity view controller is null.", @"EasyUmp");
            return;
        }

        [UMPConsentForm loadAndPresentIfRequiredFromViewController:vc
                                                 completionHandler:^(NSError * _Nullable error) {
            if (error != nil) {
                SendError(kOnShowFailed, error.code, error.localizedDescription, error.domain);
            } else {
                SendMessage(kOnShowDismissed, @"{}");
            }
        }];
    });
}

extern "C" void EasyUmpIosBridgeReshow() {
    dispatch_async(dispatch_get_main_queue(), ^{
        UIViewController* vc = UnityGetGLViewController();
        if (vc == nil) {
            SendError(kOnReshowFailed, -1, @"Unity view controller is null.", @"EasyUmp");
            return;
        }

        [UMPConsentForm presentPrivacyOptionsFormFromViewController:vc
                                                  completionHandler:^(NSError * _Nullable error) {
            if (error != nil) {
                SendError(kOnReshowFailed, error.code, error.localizedDescription, error.domain);
            } else {
                SendMessage(kOnReshowDismissed, @"{}");
            }
        }];
    });
}

extern "C" void EasyUmpIosBridgeReset() {
    dispatch_async(dispatch_get_main_queue(), ^{
        [[UMPConsentInformation sharedInstance] reset];
    });
}

extern "C" int EasyUmpIosBridgeGetConsentStatus() {
    return (int)[[UMPConsentInformation sharedInstance] consentStatus];
}

extern "C" bool EasyUmpIosBridgeCanRequestAds() {
    return [[UMPConsentInformation sharedInstance] canRequestAds];
}

static const char* CopyCString(NSString* value) {
    const char* utf8 = value ? value.UTF8String : "";
    return strdup(utf8);
}

extern "C" void EasyUmpIosBridgeFree(const char* ptr) {
    if (ptr != nullptr) {
        free((void*)ptr);
    }
}

extern "C" const char* EasyUmpIosBridgeGetTcString() {
    NSString* value = [[NSUserDefaults standardUserDefaults] stringForKey:kIabTcString];
    return CopyCString(value ?: @"");
}

extern "C" const char* EasyUmpIosBridgeGetAdditionalConsentString() {
    NSString* value = [[NSUserDefaults standardUserDefaults] stringForKey:kIabAddtlConsent];
    return CopyCString(value ?: @"");
}

extern "C" const char* EasyUmpIosBridgeGetPurposeConsentsString() {
    NSString* value = [[NSUserDefaults standardUserDefaults] stringForKey:kIabPurposeConsents];
    return CopyCString(value ?: @"");
}

extern "C" int EasyUmpIosBridgeGetGdprApplies() {
    id value = [[NSUserDefaults standardUserDefaults] objectForKey:kIabGdprApplies];
    if (value == nil) {
        return -1;
    }
    if ([value isKindOfClass:[NSNumber class]]) {
        return [(NSNumber *)value intValue];
    }
    return -1;
}
