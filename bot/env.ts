import { config } from 'dotenv'

export function loadEnv() {
  config()
  if (!process.env.BOT_TOKEN) {
    throw new Error('BOT_TOKEN не задан в .env!')
  }
}
