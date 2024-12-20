import NextAuth, {NextAuthOptions} from "next-auth";
import DuendeIDS6Provider from "next-auth/providers/duende-identity-server6"

export const authOptions: NextAuthOptions = {
    session: {
        strategy: "jwt"
    },
    providers: [
        DuendeIDS6Provider({
            id: "auth-server",
            clientId: "nextApp",
            clientSecret: "secret",
            issuer: "http://localhost:5001",
            authorization: {
                params: {
                    scope: "openid profile auctionApp"
                }
            },
            idToken: true
        })
    ],
    callbacks: {
        async jwt({token, profile, account}) {
            if(account && account.access_token)
                token.accessToken = account.access_token
            if(profile)
                token.username = profile.username
            return token
        },
        async session({session, token}){
            if(token){
                session.accessToken = token.accessToken
                session.user.username = token.username
            }
            return session;
        }
    }
}

export default NextAuth(authOptions)