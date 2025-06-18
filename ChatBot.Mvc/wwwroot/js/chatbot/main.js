import { elements, addMessage } from "/js/chatbot/ui.js";
import { sendQuestionApi } from "/js/chatbot/chatApi.js";
import { createConnection, startConnection } from "/js/chatbot/signalRClient.js";

let loadingMessageDiv = null;

function sendQuestion() {
    const question = elements.input.value.trim();
    const connectionId = connection.connectionId;
    if (!question || loadingMessageDiv) return;

    elements.sendBtn.disabled = true;

    addMessage(question, "user");
    loadingMessageDiv = addMessage("", "loading");

    sendQuestionApi(question, connectionId)
        .then(() => {
            elements.input.value = "";
        })
        .catch(err => {
            console.error("Erro:", err);
            if (loadingMessageDiv) {
                loadingMessageDiv.remove();
                loadingMessageDiv = null;
            }
        })
        .finally(() => {
            elements.sendBtn.disabled = false;
        });
}

elements.sendBtn.addEventListener("click", sendQuestion);

elements.input.addEventListener("keydown", e => {
    if (e.key === "Enter") {
        e.preventDefault();
        sendQuestion();
    }
});

const urlApi = window.urlApi

const connection = createConnection(urlApi);

connection.onclose(async () => {
    await startConnection(connection);
});

connection.on("ReceiveMessage", data => {
    console.log(data);

    if (loadingMessageDiv) {
        const bubbleBody = loadingMessageDiv.querySelector(".chat-bubble-body");
        if (bubbleBody) {
            bubbleBody.innerHTML = `<p>${data.message}</p>`;
        }
        loadingMessageDiv = null;
    } else {
        addMessage(data.message, "bot");
    }
});

startConnection(connection);