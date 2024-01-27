FROM bitnami/git:2.43.0 AS src
WORKDIR /usr/src/libsql
RUN git init \
    && git remote add --no-fetch origin "https://github.com/tursodatabase/libsql.git" \
    && git fetch --quiet origin \
    && git checkout e21cefe

# build sqld
FROM rust:slim-bullseye AS chef
RUN apt update \
    && apt install -y libclang-dev clang \
        build-essential tcl protobuf-compiler file \
        libssl-dev pkg-config git tcl \
    && apt clean \
    && rm -rf /var/lib/apt/lists/*

# We need to install and set as default the toolchain specified in rust-toolchain.toml
# Otherwise cargo-chef will build dependencies using wrong toolchain
# This also prevents planner and builder steps from installing the toolchain over and over again
COPY --from=src usr/src/libsql/rust-toolchain.toml rust-toolchain.toml
RUN cat rust-toolchain.toml | grep "channel" | awk '{print $3}' | sed 's/\"//g' > toolchain.txt \
    && rustup update $(cat toolchain.txt) \
    && rustup default $(cat toolchain.txt) \
    && rm toolchain.txt rust-toolchain.toml \
    && cargo install cargo-chef

FROM chef AS planner
COPY --from=src  /usr/src/libsql .
RUN cargo chef prepare --recipe-path recipe.json

FROM chef AS builder
COPY --from=planner /recipe.json recipe.json
RUN cargo chef cook --release --recipe-path recipe.json
COPY --from=src /usr/src/libsql .
RUN cargo build -p libsql-server --release

# runtime
FROM debian:bullseye-slim as runtime
RUN apt update

EXPOSE 5001 8080
VOLUME [ "/var/lib/sqld" ]

RUN groupadd --system --gid 666 sqld
RUN adduser --system --home /var/lib/sqld --uid 666 --gid 666 sqld
WORKDIR /var/lib/sqld
USER sqld

COPY docker-entrypoint.sh /usr/local/bin

COPY --from=builder /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/ca-certificates.crt
COPY --from=builder /target/release/sqld /bin/sqld

ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
CMD ["/bin/sqld"]

FROM debian:bullseye-slim AS download

ARG VERSION
ADD https://github.com/chiselstrike/homebrew-tap/releases/download/$VERSION/homebrew-tap_Linux_x86_64.tar.gz /tmp
RUN tar -xzf /tmp/homebrew-tap_Linux_x86_64.tar.gz -C /tmp

FROM debian:bullseye-slim AS turso
COPY --from=download /tmp/turso /bin
COPY --from=builder /etc/ssl/certs/ca-certificates.crt /etc/ssl/certs/ca-certificates.crt
COPY --from=builder /target/release/sqld /bin/sqld

ENTRYPOINT ["/bin/turso"]
