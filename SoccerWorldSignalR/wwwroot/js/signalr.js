const connection = new signalR.HubConnectionBuilder()
    .withUrl("/Hub", {
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
    })
    .withAutomaticReconnect()
    .configureLogging(signalR.LogLevel.Information)
    .build();
connection.onreconnected((connectionId) => {
    console.assert(connection.state === signalR.HubConnectionState.Connected);
});
connection.onclose((error) => {
    alert(error);
});

