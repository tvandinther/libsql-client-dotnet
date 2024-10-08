#!/usr/bin/env bash
set -euo pipefail

script_dir="$(dirname "$(readlink -f "${BASH_SOURCE[0]}")")"

export LIBSQL_RS_VERSION="0.5.0"
bindings_dir="${script_dir}/rust-bindings"
force=false

# Parse command line arguments
while [[ $# -gt 0 ]]; do
  case "$1" in
    --force)
      force=true
      shift
      ;;
    *)
      echo "Unknown option: $1"
      exit 1
      ;;
  esac
done

function run_if_force {
  if [[ "${force}" == "true" ]];then
    "$@"
  fi
}

run_if_force rm -rf "${bindings_dir}/libsql"
run_if_force rm -rf "${bindings_dir}/target"
mkdir -p "${bindings_dir}/libsql"

cd "${bindings_dir}/libsql"
git init 2> /dev/null

if ! git config remote.origin.url &> /dev/null;then
  git remote add --no-fetch origin "https://github.com/tursodatabase/libsql.git"
fi

# git sparse-checkout init
# git sparse-checkout set bindings/c bindings/wasm libsql libsql-sys
# -----
# git fetch --quiet origin
# git checkout fb85262
# -----
git fetch --depth 1 origin tag libsql-rs-v${LIBSQL_RS_VERSION}
git reset --hard tags/libsql-rs-v${LIBSQL_RS_VERSION}

cd ..
cargo build --release
