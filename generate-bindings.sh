#!/usr/bin/env bash
set -euo pipefail

script_dir="$(dirname "$(readlink -f "${BASH_SOURCE[0]}")")"

bindings_dir="${script_dir}/rust-bindings"
force=false
commit="${LIBSQL_COMMIT:-libsql-0.9.21}"
remote="${LIBSQL_REMOTE:-https://github.com/tursodatabase/libsql.git}"
local_repo_path=""

while [[ $# -gt 0 ]]; do
  case "$1" in
    --force)
      force=true
      shift
      ;;
    --commit)
      commit="$2"
      shift 2
      ;;
    --remote)
      remote="$2"
      shift 2
      ;;
    --local)
      local_repo_path="$2"
      shift 2
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
mkdir -p "${bindings_dir}"

if [[ -n "${local_repo_path}" ]]; then
  ln -sfn "${local_repo_path}" "${bindings_dir}/libsql"
else
  mkdir -p "${bindings_dir}/libsql"
  cd "${bindings_dir}/libsql"
  git init 2> /dev/null

  if ! git config remote.origin.url &> /dev/null;then
    git remote add --no-fetch origin "${remote}"
  fi

  git fetch --quiet origin ${commit} --depth 1
  git checkout FETCH_HEAD
  cd ../..
fi

cd "${bindings_dir}"
cargo build --lib --release
