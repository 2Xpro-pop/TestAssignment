using Aspire.Hosting.Yarp;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestAssignment.AppHost;

public static class YarpExtensions
{
    public static IResourceBuilder<YarpResource> ConfigureTestAssignmentRoutes(
        this IResourceBuilder<YarpResource> builder,
        IResourceBuilder<ProjectResource> identityApi,
        IResourceBuilder<ProjectResource> paymentApi,
        IResourceBuilder<ProjectResource> webFrontend)
    {
        return builder.WithConfiguration(yarp =>
        {
            var identityCluster = yarp.AddCluster(identityApi);
            var paymentCluster = yarp.AddCluster(paymentApi);
            var webFrontendCluster = yarp.AddCluster(webFrontend);

            yarp.AddRoute("/api/identity/login", identityCluster)
                .WithMatchMethods("POST");

            yarp.AddRoute("/api/identity/logout", identityCluster)
                .WithMatchMethods("POST");

            yarp.AddRoute("/api/payment", paymentCluster)
                .WithMatchMethods("POST");

            yarp.AddRoute("api/payment", paymentCluster)
                .WithMatchMethods("GET");

            yarp.AddRoute("/{**catch-all}", webFrontendCluster);
        });
    }
}