# Создаем образ для сборки
FROM oven/bun:alpine as builder
WORKDIR /app

# Сначала копируем зависимости для кэширования
COPY package.json bun.lock ./
RUN bun install --frozen-lockfile

# Копируем остальные файлы
COPY . .

# Финальный образ
FROM oven/bun:alpine
WORKDIR /app

# Копируем только необходимое из builder
COPY --from=builder /app/node_modules ./node_modules
COPY --from=builder /app/package.json ./
COPY --from=builder /app/bot ./bot
COPY --from=builder /app/shared ./shared
# Добавьте другие необходимые директории

# Устанавливаем окружение
ENV NODE_ENV=production

# Запускаем от непривилегированного пользователя
RUN addgroup -S appgroup && adduser -S appuser -G appgroup
USER appuser

# Health check
HEALTHCHECK --interval=30s --timeout=30s --start-period=5s --retries=3 \
  CMD wget --no-verbose --tries=1 --spider http://localhost:3000/health || exit 1

CMD ["bun", "run", "bot/bot.ts"]