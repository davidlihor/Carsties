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
  output: "standalone",

  async rewrites() {
    return [
      {
        source: "/metrics",
        destination: "/api/metrics"
      }
    ]
  }
};

export default nextConfig;
