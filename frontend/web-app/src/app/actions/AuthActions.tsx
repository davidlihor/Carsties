import { getServerSession } from "next-auth";
import {authOptions} from "@/auth";

export async function getCurrentSession() {
    try {
        const session = await getServerSession(authOptions);
        if (session) return session
        return null;
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
    } catch (error) {
        return null;
    }
}