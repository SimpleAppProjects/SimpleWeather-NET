﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Foundation;
using RevenueCat;

namespace Xamarin.RevenueCat.iOS.Extensions
{
    // ReSharper disable once InconsistentNaming
    public static class RCPurchasesExtensions
    {
        public static Task<LoginResult> LoginAsync(this RCPurchases purchases, string appUserId,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<LoginResult>();
            cancellationToken.Register(() => tcs.TrySetCanceled());
            purchases.LogIn(appUserId, (customerInfo, created, error) =>
            {
                if (error != null)
                {
                    tcs.TrySetException(new PurchasesErrorException(error, false));
                }
                else
                {
                    tcs.TrySetResult(new LoginResult(customerInfo, created));
                }
            });
            return tcs.Task;
        }

        public static Task<RCCustomerInfo> LogOutAsync(this RCPurchases purchases,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<RCCustomerInfo>();
            cancellationToken.Register(() => tcs.TrySetCanceled());
            purchases.LogOutWithCompletion((customerInfo, error) =>
            {
                if (error != null)
                {
                    tcs.TrySetException(new PurchasesErrorException(error, false));
                }
                else
                {
                    tcs.TrySetResult(customerInfo);
                }
            });
            return tcs.Task;
        }

        public static Task<RCOfferings> GetOfferingsAsync(this RCPurchases purchases,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<RCOfferings>();
            cancellationToken.Register(() => tcs.TrySetCanceled());
            purchases.GetOfferingsWithCompletion((RCOfferings offerings, NSError error) =>
            {
                if (error != null)
                {
                    tcs.TrySetException(new PurchasesErrorException(error, false));
                }
                else
                {
                    tcs.TrySetResult(offerings);
                }
            });
            return tcs.Task;
        }

        public static Task<PurchaseSuccessInfo> PurchasePackageAsync(this RCPurchases purchases,
            RCPackage packageToPurchase, CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<PurchaseSuccessInfo>();
            cancellationToken.Register(() => tcs.TrySetCanceled());
            purchases.PurchasePackage(packageToPurchase,
                (RCStoreTransaction transaction, RCCustomerInfo customerInfo, NSError error, bool userCancelled) =>
                {
                    if (error != null)
                    {
                        tcs.TrySetException(new PurchasesErrorException(error, userCancelled));
                    }
                    else if (userCancelled)
                    {
                        tcs.TrySetException(new PurchasesErrorException(null, true));
                    }
                    else
                    {
                        tcs.TrySetResult(new PurchaseSuccessInfo(transaction, customerInfo));
                    }
                });
            return tcs.Task;
        }

        public static Task<RCCustomerInfo> RestorePurchasesAsync(this RCPurchases purchases,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<RCCustomerInfo>();
            cancellationToken.Register(() => tcs.TrySetCanceled());
            purchases.RestorePurchasesWithCompletion((RCCustomerInfo customerInfo, NSError error) =>
            {
                if (error != null)
                {
                    tcs.TrySetException(new PurchasesErrorException(error, false));
                }
                else
                {
                    tcs.TrySetResult(customerInfo);
                }
            });
            return tcs.Task;
        }

        [Obsolete("Obsolete due to confusing naming. Use RestorePurchasesAsync instead.")]
        public static Task<RCCustomerInfo> RestoreTransactionsAsync(this RCPurchases purchases,
            CancellationToken cancellationToken = default)
        {
            return RestorePurchasesAsync(purchases, cancellationToken);
        }

        public static Task<RCCustomerInfo> GetCustomerInfoAsync(this RCPurchases purchases,
            CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<RCCustomerInfo>();
            cancellationToken.Register(() => tcs.TrySetCanceled());
            purchases.GetCustomerInfoWithCompletion((RCCustomerInfo customerInfo, NSError error) =>
            {
                if (error != null)
                {
                    tcs.TrySetException(new PurchasesErrorException(error, false));
                }
                else
                {
                    tcs.TrySetResult(customerInfo);
                }
            });
            return tcs.Task;
        }
    }
}
