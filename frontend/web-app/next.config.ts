import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  logging: {
    fetches: {
      fullUrl: true
    }
  },
  images: {
    remotePatterns: [
      { protocol: "http", hostname: "*" }
    ]
  },
  output: "standalone"
};

export default nextConfig;
