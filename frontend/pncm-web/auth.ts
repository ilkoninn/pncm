import NextAuth from "next-auth";
import Credentials from "next-auth/providers/credentials";

function parseJwt(token: string) {
  const base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
  return JSON.parse(Buffer.from(base64, "base64").toString());
}

export const { handlers, auth, signIn, signOut } = NextAuth({
  session: { strategy: "jwt" },
  providers: [
    Credentials({
      credentials: {
        accessToken: { type: "text" },
        refreshToken: { type: "text" },
        expiresAt: { type: "text" },
      },
      async authorize(credentials) {
        if (!credentials.accessToken) return null;

        const payload = parseJwt(credentials.accessToken as string);

        const nameId =
          payload[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
          ] ?? payload.sub;
        const email =
          payload[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          ] ?? payload.email;
        const firstName =
          payload[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
          ] ?? "";

        return {
          id: nameId,
          email,
          username: firstName,
          accessToken: credentials.accessToken as string,
          refreshToken: credentials.refreshToken as string,
          expiresAt: credentials.expiresAt as string,
        };
      },
    }),
  ],
  callbacks: {
    async jwt({ token, user }) {
      if (user) {
        token.accessToken = user.accessToken;
        token.refreshToken = user.refreshToken;
        token.userId = user.id as string;
        token.username = user.username;
        token.roles = [];
        token.accessTokenExpires = new Date(
          user.expiresAt as string
        ).getTime();
      }
      return token;
    },
    async session({ session, token }) {
      session.accessToken = token.accessToken as string;
      session.refreshToken = token.refreshToken as string;
      session.userId = token.userId as string;
      session.username = token.username as string;
      session.roles = token.roles as string[];
      session.accessTokenExpires = token.accessTokenExpires as number;
      if (session.user) {
        session.user.email = token.email as string;
      }
      return session;
    },
  },
  pages: {
    signIn: "/login",
  },
});
