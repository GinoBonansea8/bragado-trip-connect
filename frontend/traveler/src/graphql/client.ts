import { ApolloClient, InMemoryCache, HttpLink } from '@apollo/client'

// In Docker, nginx proxies /graphql to the backend container (see nginx.conf).
// For `npm run dev` outside Docker, set VITE_GRAPHQL_URL to the backend's URL.
const graphqlUri = import.meta.env.VITE_GRAPHQL_URL ?? '/graphql'

export const client = new ApolloClient({
  link: new HttpLink({ uri: graphqlUri }),
  cache: new InMemoryCache(),
})
