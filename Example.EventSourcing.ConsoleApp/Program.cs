﻿using Cocona;
using Example.EventSourcing.ConsoleApp.Domain;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var builder = CoconaApp.CreateBuilder();
builder.Logging.AddDebug();
builder.Services.AddTransient<WarehouseProductRepository>();

var app = builder.Build();

app.AddCommand("ship", async ([Option] string sku, [Option] int quantity, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var product = await repository.GetAsync(sku, ctx.CancellationToken);
    product.ShipProduct(quantity);
    await repository.SaveAsync(product, ctx.CancellationToken);
    
    logger.LogInformation("Product Sku: {0}. Shipped {1} item(s). Current quantity: {2}", product.Sku, quantity, product.GetQuantity());
});


app.AddCommand("receive", async ([Option] string sku, [Option] int quantity, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var product = await repository.GetAsync(sku, ctx.CancellationToken);
    product.ReceiveProduct(quantity);
    await repository.SaveAsync(product, ctx.CancellationToken);
    
    logger.LogInformation("Product Sku: {0}. Recieved {1} item(s). Current quantity: {2}", product.Sku, quantity, product.GetQuantity());
});


app.AddCommand("setQuantity", async ([Option] string sku, [Option] int quantity, [Option] string reason, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var product = await repository.GetAsync(sku, ctx.CancellationToken);
    product.AdjustInventory(quantity, reason);
    await repository.SaveAsync(product, ctx.CancellationToken);
    
    logger.LogInformation("Product Sku: {0}. Adjusted quantity to {1} item(s). Current quantity: {2}", product.Sku, quantity, product.GetQuantity());
});


app.AddCommand("getQuantity", async ([Option] string sku, WarehouseProductRepository repository, ILogger<Program> logger, CoconaAppContext ctx) =>
{
    var product = await repository.GetAsync(sku, ctx.CancellationToken);
    logger.LogInformation("Product Sku: {0}. Current quantity: {1}", product.Sku, product.GetQuantity());
});


app.Run();