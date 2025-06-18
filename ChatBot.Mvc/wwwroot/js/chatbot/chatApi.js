export function sendQuestionApi(question, connectionId) {
    return axios.get(`/Home/Chat`, { params: { question, connectionId } });
}