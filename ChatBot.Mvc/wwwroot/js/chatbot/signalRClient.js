export function createConnection(urlApi) {
    Object.defineProperty(WebSocket, 'OPEN', { value: 1 });

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`${urlApi}/chatHub`)
        .configureLogging(signalR.LogLevel.Trace)
        .withAutomaticReconnect()
        .build();

    return connection;
}

export async function startConnection(connection) {
    try {
        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(() => startConnection(connection), 5000);
    }
}