export const elements = {
    input: document.getElementById("questionInput"),
    chatContainer: document.getElementById("chatContainer"),
    sendBtn: document.getElementById("sendBtn"),
};

export let loadingMessageDiv = null;

export function addMessage(text, sender) {
    const messageDiv = document.createElement("div");
    messageDiv.classList.add("chat-item");

    if (sender === "user") {
        messageDiv.innerHTML = `
            <div class="row align-items-end justify-content-end">
                <div class="col col-lg-6">
                    <div class="chat-bubble chat-bubble-me">
                        <div class="chat-bubble-body">
                            <p>${text}</p>
                        </div>
                    </div>
                </div>
            </div>
        `;
    } else if (sender === "loading") {
        messageDiv.innerHTML = `
            <div class="row align-items-end">
                <div class="col col-lg-6">
                    <div class="chat-bubble">
                        <div class="chat-bubble-body d-flex align-items-center p-2">
                            <div class="spinner-border spinner-border-sm text-primary me-2" role="status">
                                <span class="visually-hidden">Loading...</span>
                            </div>
                            <span>Carregando...</span>
                        </div>
                    </div>
                </div>
            </div>
        `;
    } else {
        messageDiv.innerHTML = `
            <div class="row align-items-end">
                <div class="col col-lg-6">
                    <div class="chat-bubble">
                        <div class="chat-bubble-body">
                            <p>${text}</p>
                        </div>
                    </div>
                </div>
            </div>
        `;
    }

    elements.chatContainer.appendChild(messageDiv);
    return messageDiv;
}
