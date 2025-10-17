import NextAuth, { Profile } from "next-auth";
import { OIDCConfig } from "next-auth/providers"
import Keycloak from "next-auth/providers/keycloak"

export const { handlers, auth } = NextAuth({
    session: {
        strategy: "jwt"
    },
    providers: [
        Keycloak({
            id: "auth-server",
            clientId: process.env.CLIENT_ID,
            clientSecret: process.env.CLIENT_SECRET,
            issuer: `${process.env.ID_URL}/realms/${process.env.REALM}`,
            authorization: {
                params: { scope: "openid profile email" },
                url: `${process.env.ID_URL}/realms/${process.env.REALM}/protocol/openid-connect/auth`
            },
            token: {
                url: `${process.env.ID_URL_INTERNAL}/realms/${process.env.REALM}/protocol/openid-connect/token`
            },
            userinfo: {
                url: `${process.env.ID_URL_INTERNAL}/realms/${process.env.REALM}/protocol/openid-connect/userinfo`
            },
            idToken: true
        } as OIDCConfig<Profile>)
    ],
    callbacks: {
        async redirect({ url, baseUrl }) {
            return url.startsWith(baseUrl) ? url : baseUrl
        },
        async authorized({ auth }) {
            return !!auth
        },
        async jwt({ token, profile, account }) {
            if (account && account.access_token)
                token.accessToken = account.access_token
            if (profile && profile.preferred_username)
                token.username = profile.preferred_username
            return token
        },
        async session({ session, token }) {
            if (token) {
                session.accessToken = token.accessToken
                session.user.username = token.username
            }
            return session;
        }
    }
})
