"use server"
import { auth } from "@/auth";

export async function getCurrentSession() {
    try {
        const session = await auth();
        if (session) return session
        return null;
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
    } catch (error) {
        return null;
    }
}