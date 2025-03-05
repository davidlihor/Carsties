declare global {
    namespace NodeJS {
        interface ProcessEnv {
            AUTH_SECRET: string
            AUTH_URL: string
            API_URL: string
            ID_URL: string
            ID_URL_INTERNAL: string
            NOTIFY_URL: string
            NODE_TLS_REJECT_UNAUTHORIZED: number

            CLIENT_ID: string
            CLIENT_SECRET: string
            REALM: string
        }
    }
}

export { };