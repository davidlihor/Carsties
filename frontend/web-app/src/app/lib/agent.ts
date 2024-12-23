import axios, {AxiosError, AxiosResponse} from "axios";
import {getCurrentSession} from "@/app/actions/AuthActions";

const agent = axios.create({
    baseURL: "http://localhost:7002",
    timeout: 5000,
});
agent.interceptors.request.use(async request => {
    const session = await getCurrentSession();
    request.headers.Authorization = `Bearer ${session?.accessToken}`
    return request
})
agent.interceptors.response.use(async response => {
    return response;
}, (error: AxiosError) => {
    const customError = {
        code: error.response?.status || 500,
        status: error.response?.statusText,
        message: error.message || "Something went wrong",
        response: error.response?.data || null
    }
    console.log("Axios Error:", customError);
    return Promise.reject(customError);
});

async function requestWrapper<T>(promise: Promise<AxiosResponse<T>> ): Promise<T> {
    try {
        const response: AxiosResponse<T> = await promise;
        return response.data;
    }catch (error) {
        throw error;
    }
}

const agentMethods = {
    async get<T>(url: string){
        return requestWrapper(agent.get<T>(url));
    },
    async post<T>(url: string, body: object){
        return requestWrapper(agent.post<T>(url, body));
    },
    async put<T>(url: string, body: object){
        return requestWrapper(agent.put<T>(url, body));
    },
    async delete<T>(url: string){
        return requestWrapper(agent.delete<T>(url));
    },
}

export default agentMethods;