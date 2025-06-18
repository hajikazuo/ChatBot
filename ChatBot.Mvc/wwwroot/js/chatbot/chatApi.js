export function sendQuestionApi(question) {
    return axios.get(`/Home/Chat`, { params: { question } });
}