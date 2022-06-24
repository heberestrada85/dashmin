FROM mcr.microsoft.com/dotnet/sdk:3.1
LABEL maintainer="Heber Estrada <heber.estrada@outlook.com>"

ENV PATH $PATH:/root/.dotnet/tools

ARG USERNAME=developer
ARG USER_UID=1000
ARG USER_GID=$USER_UID
WORKDIR /workspace

RUN apt-get update -qq && \
    apt-get install -yq --no-install-recommends \
    nano sudo postgresql-client-11 less && \
    rm -rf /var/lib/apt/lists/* && \
    groupadd --gid $USER_GID $USERNAME && \
    useradd -s /bin/bash --uid $USER_UID --gid $USER_GID -m $USERNAME && \
    echo "$USERNAME ALL=(root) NOPASSWD:ALL" > /etc/sudoers.d/$USERNAME && \
    chmod 0440 /etc/sudoers.d/$USERNAME && \
    curl -LO "https://dl.k8s.io/release/$(curl -L -s https://dl.k8s.io/release/stable.txt)/bin/linux/amd64/kubectl" && \
    install -o root -g root -m 0755 kubectl /usr/local/bin/kubectl && \
    curl -fsSL -o get_helm.sh https://raw.githubusercontent.com/helm/helm/main/scripts/get-helm-3 && \
    chmod 700 get_helm.sh && \
    ./get_helm.sh && \
    helm repo add bitnami https://charts.bitnami.com/bitnami && \
    dotnet tool install --global dotnet-ef

RUN dotnet ef --version

COPY path-profile.sh /etc/profile.d/
